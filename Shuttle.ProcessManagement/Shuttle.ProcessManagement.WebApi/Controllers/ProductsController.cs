using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Shuttle.Core.Contract;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductQuery _productQuery;

        public ProductsController(IProductQuery productQuery)
        {
            Guard.AgainstNull(productQuery, nameof(productQuery));

            _productQuery = productQuery;
        }

        public dynamic Get()
        {
            return _productQuery.All()
                .Select(row => new
                {
                    Id = ProductColumns.Id.MapFrom(row),
                    Description = ProductColumns.Description.MapFrom(row),
                    Price = ProductColumns.Price.MapFrom(row)
                });
        }
    }
}