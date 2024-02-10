using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessCancelled
    {
        public Guid OrderProcessId { get; set; }
    }
}