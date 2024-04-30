using System;

namespace Shuttle.ProcessManagement.Domain
{
    public interface IOrderProcessService
    {
        dynamic ActiveOrders();
        void CancelOrder(Guid id);
        void ArchiveOrder(Guid id);
    }
}