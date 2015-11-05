using System;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessRepository
    {
        void Add(OrderProcess orderProcess);
        OrderProcess Get(Guid id);
        void Remove(OrderProcess orderProcess);
        void AddStatus(OrderProcessStatus orderProcessStatus, Guid id);
        void SaveOrderId(Guid orderId, Guid id);
        void SaveInvoiceId(Guid invoiceId, Guid id);
    }
}