using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
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
            return _orderProcessService.ActiveOrders();
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

            var message = new RegisterOrderProcessCommand
            {
                CustomerName = model.CustomerName,
                CustomerEMail = model.CustomerEMail,
                TargetSystem = model.TargetSystem
            };

            switch (model.TargetSystem.ToLower(CultureInfo.InvariantCulture))
            {
                case "custom":
                {
                    message.TargetSystemUri = "rabbitmq://shuttle:shuttle!@localhost/process-custom-server";

                    break;
                }
                case "custom / event-source":
                {
                    message.TargetSystemUri = "rabbitmq://shuttle:shuttle!@localhost/process-custom-es-server";

                    break;
                }
                case "event-source / module":
                {
                    message.TargetSystemUri = "rabbitmq://shuttle:shuttle!@localhost/process-es-module-server";

                    break;
                }
                default:
                {
                    throw new ApplicationException(string.Format("Unknown target system '{0}'.", model.TargetSystem));
                }
            }

            foreach (var productIdValue in model.ProductIds)
            {
                if (!Guid.TryParse(productIdValue, out var productId))
                {
                    throw new ArgumentException(string.Format("Product id '{0}' is not a valid guid.", productIdValue));
                }

                var productRow = _productQuery.Get(productId);

                message.QuotedProducts.Add(new QuotedProduct
                {
                    ProductId = productId,
                    Description = ProductColumns.Description.MapFrom(productRow),
                    Price = ProductColumns.Price.MapFrom(productRow)
                });
            }

            _bus.Send(message, c =>
            {
                c.WithRecipient(message.TargetSystemUri);
                c.Headers.Add(new TransportHeader
                {
                    Key = "TargetSystem",
                    Value = message.TargetSystem
                });
            });
        }
    }
}