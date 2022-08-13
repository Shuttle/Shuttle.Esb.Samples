using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class CancelOrderProcess
    {
        public Guid OrderProcessId { get; set; }
    }
}