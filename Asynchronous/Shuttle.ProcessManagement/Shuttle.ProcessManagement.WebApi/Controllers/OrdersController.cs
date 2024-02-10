using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly IServiceBus _bus;
        private readonly IOrderProcessService _orderProcessService;
        private readonly IProductQuery _productQuery;

        public OrdersController(IServiceBus bus, IOrderProcessService orderProcessService, IProductQuery productQuery)
        {
            Guard.AgainstNull(bus, nameof(bus));
            Guard.AgainstNull(orderProcessService, nameof(orderProcessService));
            Guard.AgainstNull(productQuery, nameof(productQuery));

            _bus = bus;
            _orderProcessService = orderProcessService;
            _productQuery = productQuery;
        }

        [HttpGet]
        public dynamic Get()
        {
            return new { Data = _orderProcessService.ActiveOrders() };
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _orderProcessService.CancelOrder(id);
        }

        [HttpPost]
        public void Post([FromBody] RegisterOrderModel model)
        {
            Guard.AgainstNull(model, nameof(model));
            Guard.AgainstNull(model.TargetSystem, nameof(model.TargetSystem));
            Guard.Against<Exception>(model.ProductIds.Count == 0, "No products have been selected.");

            var message = new RegisterOrderProcess
            {
                CustomerName = model.CustomerName,
                CustomerEMail = model.CustomerEMail,
                TargetSystem = model.TargetSystem
            };

            switch (model.TargetSystem.ToLower(CultureInfo.InvariantCulture))
            {
                case "custom":
                {
                    message.TargetSystemUri = "azuresq://azure/process-custom-server";

                    break;
                }
                case "custom / event-source":
                {
                    message.TargetSystemUri = "azuresq://azure/process-custom-es-server";

                    break;
                }
                case "event-source / module":
                {
                    message.TargetSystemUri = "azuresq://azure/process-es-module-server";

                    break;
                }
                default:
                {
                    throw new ApplicationException($"Unknown target system '{model.TargetSystem}'.");
                }
            }

            foreach (var productIdValue in model.ProductIds)
            {
                if (!Guid.TryParse(productIdValue, out var productId))
                {
                    throw new ArgumentException($"Product id '{productIdValue}' is not a valid guid.");
                }

                var productRow = _productQuery.Get(productId);

                message.QuotedProducts.Add(new QuotedProduct
                {
                    ProductId = productId,
                    Description = ProductColumns.Description.MapFrom(productRow),
                    Price = ProductColumns.Price.MapFrom(productRow)
                });
            }

            _bus.Send(message, builder =>
            {
                builder.WithRecipient(message.TargetSystemUri);
                builder.Headers.Add(new TransportHeader
                {
                    Key = "TargetSystem",
                    Value = message.TargetSystem
                });
            });
        }
    }
}