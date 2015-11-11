namespace Shuttle.Ordering.Domain
{
    public class OrderItem
    {
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        public OrderItem(string description, decimal price)
        {
            Description = description;
            Price = price;
        }
    }
}