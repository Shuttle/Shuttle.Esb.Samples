using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class ArchiveOrderProcessRejected
    {
        public Guid OrderProcessId { get; set; }
        public string Status { get; set; }
    }
}