using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessArchivedEvent
    {
        public Guid OrderProcessId { get; set; }
    }
}