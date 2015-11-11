using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessCompletedEvent
    {
        public Guid OrderProcessId { get; set; }
    }
}