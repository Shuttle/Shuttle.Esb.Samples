using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class CancelOrderProcessCommand
    {
        public Guid OrderProcessId { get; set; }
    }
}