using System;

namespace Shuttle.ProcessManagement.Events.v1
{
    public class Initialized
    {
        public string OrderNumber { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}