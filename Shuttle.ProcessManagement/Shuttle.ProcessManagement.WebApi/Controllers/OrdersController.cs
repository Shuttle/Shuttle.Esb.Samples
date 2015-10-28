using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class OrdersController : ShuttleApiController
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrdersController(IDatabaseContextFactory  databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
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
        }
    }
}