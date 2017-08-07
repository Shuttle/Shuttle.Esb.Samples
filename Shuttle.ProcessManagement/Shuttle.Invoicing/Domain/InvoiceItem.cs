namespace Shuttle.Invoicing.Domain
{
    public class InvoiceItem
    {
        public InvoiceItem(string description, decimal price)
        {
            Description = description;
            Price = price;
        }

        public string Description { get; }
        public decimal Price { get; }
    }
}