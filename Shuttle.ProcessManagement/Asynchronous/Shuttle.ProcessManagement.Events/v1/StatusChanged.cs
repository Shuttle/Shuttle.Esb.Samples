using System;

namespace Shuttle.ProcessManagement.Events.v1
{
    public class StatusChanged
    {
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}