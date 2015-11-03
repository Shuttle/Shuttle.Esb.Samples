using System;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessRepository
    {
        void Add(OrderProcess orderProcess);
        OrderProcess Get(Guid id);
        void Remove(OrderProcess orderProcess);
    }
}