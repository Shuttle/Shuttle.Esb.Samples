namespace Shuttle.Ordering.Domain
{
    public class OrderItem
    {
        public OrderItem(string description, decimal price)
        {
            Description = description;
            Price = price;
        }

        public string Description { get; }
        public decimal Price { get; }
    }
}