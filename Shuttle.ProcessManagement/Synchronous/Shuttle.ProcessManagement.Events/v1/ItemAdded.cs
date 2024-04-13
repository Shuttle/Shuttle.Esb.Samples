using System;

namespace Shuttle.ProcessManagement.Events.v1
{
    public class ItemAdded
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}