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

        public Guid ProductId { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
    }
}