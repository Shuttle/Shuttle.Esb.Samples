using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class OrdersController : ShuttleApiController
    {
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrdersController(IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _orderProcessViewQuery = orderProcessViewQuery;
        }

        // GET api/values
        public HttpResponseMessage Get()
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

        // POST api/values
        public void Post([FromBody] RegisterOrderModel model)
        {
            Guard.AgainstNull(model, "model");
        }
    }
}