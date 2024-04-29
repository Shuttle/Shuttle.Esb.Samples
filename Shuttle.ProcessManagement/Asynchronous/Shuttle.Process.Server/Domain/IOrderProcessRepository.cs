using System;
using System.Threading.Tasks;
using Shuttle.Process.Custom.Server.Domain;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessRepository
    {
        Task AddAsync(OrderProcess orderProcess);
        Task<OrderProcess> GetAsync(Guid id);
        Task RemoveAsync(OrderProcess orderProcess);
        Task AddStatusAsync(OrderProcessStatus orderProcessStatus, Guid id);
        Task SaveOrderIdAsync(Guid orderId, Guid id);
        Task SaveInvoiceIdAsync(Guid invoiceId, Guid id);
    }
}