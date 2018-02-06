using System;
using Microsoft.AspNetCore.Mvc;
using Shuttle.Core.Contract;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class ArchivedOrdersController : Controller
    {
        private readonly IOrderProcessService _orderProcessService;

        public ArchivedOrdersController(IOrderProcessService orderProcessService)
        {
            Guard.AgainstNull(orderProcessService, nameof(orderProcessService));

            _orderProcessService = orderProcessService;
        }

        [HttpPost]
        [Route("api/[controller]/{id}")]
        public void Post(Guid id)
        {
            _orderProcessService.ArchiveOrder(id);
        }
    }
}