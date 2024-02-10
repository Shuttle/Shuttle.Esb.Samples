using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessCompleted
    {
        public Guid OrderProcessId { get; set; }
    }
}