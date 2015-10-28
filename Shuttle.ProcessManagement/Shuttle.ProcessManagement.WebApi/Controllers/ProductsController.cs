using System.Linq;
using System.Net.Http;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class ProductsController : ShuttleApiController
    {
        private readonly IProductQuery _productQuery;

        public ProductsController(IProductQuery productQuery)
        {
            Guard.AgainstNull(productQuery, "productQuery");

            _productQuery = productQuery;
        }

        public HttpResponseMessage Get()
        {
            return OK(from row in _productQuery.All()
                select new
                {
                    Id = ProductColumns.Id.MapFrom(row),
                    Description = ProductColumns.Description.MapFrom(row),
                    Price = ProductColumns.Price.MapFrom(row)
                });
        }
    }
}