using System.Threading.Tasks;

namespace Shuttle.Invoicing.Domain
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);
    }
}