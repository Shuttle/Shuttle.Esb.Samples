using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class ArchiveOrderProcessRejectedEvent
    {
        public Guid OrderProcessId { get; set; }
        public string Status { get; set; }
    }
}