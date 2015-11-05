using System;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessQueryFactory
    {
        IQuery Add(OrderProcess orderProcess);
        IQuery AddItem(OrderProcessItem orderProcessItem, Guid id);
        IQuery AddStatus(OrderProcessStatus status, Guid id);
        IQuery Get(Guid id);
        IQuery GetItems(Guid id);
        IQuery GetStatuses(Guid id);
        IQuery Remove(Guid id);
        IQuery SaveOrderId(Guid orderId, Guid id);
        IQuery SaveInvoiceId(Guid invoiceId, Guid id);
    }
}