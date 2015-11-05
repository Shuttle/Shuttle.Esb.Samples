namespace Shuttle.Invoicing.Domain
{
    public class InvoiceItem
    {
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        public InvoiceItem(string description, decimal price)
        {
            Description = description;
            Price = price;
        }
    }
}