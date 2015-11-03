using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class OrdersController : ShuttleApiController
    {
        private readonly IServiceBus _bus;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessService _orderProcessService;
        private readonly IProductQuery _productQuery;

        public OrdersController(IServiceBus bus, IDatabaseContextFactory databaseContextFactory, IOrderProcessService orderProcessService, IProductQuery productQuery)
        {
            Guard.AgainstNull(bus, "bus");
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessService, "OrderProcessService");
            Guard.AgainstNull(productQuery, "productQuery");

            _bus = bus;
            _databaseContextFactory = databaseContextFactory;
            _orderProcessService = orderProcessService;
            _productQuery = productQuery;
        }

        public HttpResponseMessage Get()
        {
            return OK(_orderProcessService.ActiveOrders());
        }

        public HttpResponseMessage Delete(Guid id)
        {
            _orderProcessService.CancelOrder(id);

            return OK();
        }

        public void Post([FromBody] RegisterOrderModel model)
        {
            Guard.AgainstNull(model, "model");
            Guard.AgainstNull(model.TargetSystem, "model.TargetSystem");
            Guard.Against<Exception>(model.ProductIds.Count == 0, "No products have been selected.");

            var message = new RegisterOrderProcessCommand
            {
                CustomerName = model.CustomerName,
                CustomerEMail = model.CustomerEMail,
                TargetSystem = model.TargetSystem
            };

            switch (model.TargetSystem.ToLower(CultureInfo.InvariantCulture))
            {
                case "handrolled":
                    {
                        message.TargetSystemUri = "msmq://./process-handrolled-server";

                        break;
                    }
                case "defaultmodule":
                    {
                        message.TargetSystemUri = "msmq://./process-default-server";

                        break;
                    }
                default:
                    {
                        throw new ApplicationException(string.Format("Unknown target system '{0}'.", model.TargetSystem));
                    }
            }

            foreach (var productIdValue in model.ProductIds)
            {
                Guid productId;

                if (!Guid.TryParse(productIdValue, out productId))
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

            _bus.Send(message, c => { c.WithRecipient(message.TargetSystemUri); });
        }
    }
}