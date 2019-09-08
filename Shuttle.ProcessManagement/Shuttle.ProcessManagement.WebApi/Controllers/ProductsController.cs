using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Shuttle.Core.Contract;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductQuery _productQuery;

        public ProductsController(IProductQuery productQuery)
        {
            Guard.AgainstNull(productQuery, nameof(productQuery));

            _productQuery = productQuery;
        }

        [HttpGet]
        public dynamic Get()
        {
            return new
            {
                Data = _productQuery.All()
                    .Select(row => new
                    {
                        Id = ProductColumns.Id.MapFrom(row),
                        Description = ProductColumns.Description.MapFrom(row),
                        Price = ProductColumns.Price.MapFrom(row),
                        Url = ProductColumns.Url.MapFrom(row)
                    })
            };
        }
    }
}