using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessAcceptedEvent
    {
        public Guid OrderProcessId { get; set; }
    }
}