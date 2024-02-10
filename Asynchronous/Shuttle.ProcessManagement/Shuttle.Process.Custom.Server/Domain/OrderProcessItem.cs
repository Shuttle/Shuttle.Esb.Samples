using System;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessItem
    {
        public OrderProcessItem(Guid productId, string description, decimal price)
        {
            ProductId = productId;
            Description = description;
            Price = price;
        }

        public Guid ProductId { get; }
        public string Description { get; }
        public decimal Price { get; }
    }
}