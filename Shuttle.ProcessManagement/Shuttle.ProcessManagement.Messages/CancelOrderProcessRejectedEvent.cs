using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class CancelOrderProcessRejectedEvent
    {
        public Guid OrderProcessId { get; set; }
        public string Status { get; set; }
    }
}