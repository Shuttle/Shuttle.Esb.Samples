namespace Shuttle.Invoicing.Domain
{
    public interface IInvoiceRepository
    {
        void Add(Invoice invoice);
    }
}