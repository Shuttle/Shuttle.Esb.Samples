using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class CompleteOrderProcessCommand
    {
        public Guid OrderProcessId { get; set; } 
    }
}