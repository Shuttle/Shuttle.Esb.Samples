using System;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessService
    {
        dynamic ActiveOrders();
        void CancelOrder(Guid id);
    }
}