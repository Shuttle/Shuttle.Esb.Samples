using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class CancelOrderProcessRejected
    {
        public Guid OrderProcessId { get; set; }
        public string Status { get; set; }
    }
}