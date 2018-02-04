using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class ArchivedOrdersController : ShuttleApiController
    {
        private readonly IOrderProcessService _orderProcessService;

        public ArchivedOrdersController(IOrderProcessService orderProcessService)
        {
            Guard.AgainstNull(orderProcessService, "orderProcessService");

            _orderProcessService = orderProcessService;
        }

        public void Post(Guid id)
        {
            _orderProcessService.ArchiveOrder(id);
        }
    }
}