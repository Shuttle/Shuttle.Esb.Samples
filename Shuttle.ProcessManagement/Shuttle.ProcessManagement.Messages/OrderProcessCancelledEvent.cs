using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessCancelledEvent
    {
        public Guid OrderProcessId { get; set; }
    }
}