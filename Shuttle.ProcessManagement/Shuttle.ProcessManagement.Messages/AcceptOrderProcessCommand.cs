using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class AcceptOrderProcessCommand
    {
        public Guid OrderProcessId { get; set; }
    }
}