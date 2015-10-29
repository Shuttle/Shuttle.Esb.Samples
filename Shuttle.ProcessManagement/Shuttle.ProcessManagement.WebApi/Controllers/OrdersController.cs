using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;
        private readonly IProductQuery _productQuery;

        public OrdersController(IServiceBus bus, IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery, IProductQuery productQuery)
        {
            Guard.AgainstNull(bus, "bus");
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");
            Guard.AgainstNull(productQuery, "productQuery");

            _bus = bus;
            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
            _productQuery = productQuery;
        }

        public HttpResponseMessage Get()
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                return OK(from row in _orderProcessViewQuery.All()
                          select new
                          {
                              Id = OrderProcessViewColumns.Id.MapFrom(row),
                              OrderNumber = OrderProcessViewColumns.OrderNumber.MapFrom(row),
                              OrderDate = OrderProcessViewColumns.OrderDate.MapFrom(row),
                              Status = OrderProcessViewColumns.Status.MapFrom(row)
                          });
            }
        }

        public void Post([FromBody] RegisterOrderModel model)
        {
            Guard.AgainstNull(model, "model");
            Guard.AgainstNull(model.TargetSystem, "model.TargetSystem");
            Guard.Against<Exception>(model.ProductIds.Count == 0, "No products have been selected.");

            var message = new RegisterOrderProcessCommand();

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

            switch (model.TargetSystem.ToLower(CultureInfo.InvariantCulture))
            {
                case "handrolled":
                    {
                        _bus.Send(message, c => { c.WithRecipient("msmq://./process-handrolled-server"); });

                        break;
                    }
                case "defaultmodule":
                    {
                        _bus.Send(message, c => { c.WithRecipient("msmq://./process-default-server"); });

                        break;
                    }
                default:
                    {
                        throw new ApplicationException(string.Format("Unknown target system '{0}'.", model.TargetSystem));
                    }
            }
        }
    }
}