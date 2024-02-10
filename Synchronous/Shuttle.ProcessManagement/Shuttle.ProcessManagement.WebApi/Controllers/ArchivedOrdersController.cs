using System;
using Microsoft.AspNetCore.Mvc;
using Shuttle.Core.Contract;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    [Route("[controller]")]
    public class ArchivedOrdersController : Controller
    {
        private readonly IOrderProcessService _orderProcessService;

        public ArchivedOrdersController(IOrderProcessService orderProcessService)
        {
            Guard.AgainstNull(orderProcessService, nameof(orderProcessService));

            _orderProcessService = orderProcessService;
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _orderProcessService.ArchiveOrder(id);
        }
    }
}