using System;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessQueryFactory
    {
        IQuery Add(OrderProcess orderProcess);
        IQuery AddItem(OrderProcessItem orderProcessItem, Guid orderProcessId);
        IQuery AddStatus(OrderProcessStatus status, Guid orderProcessId);
        IQuery Get(Guid id);
        IQuery GetItems(Guid id);
        IQuery GetStatuses(Guid id);
        IQuery Remove(Guid id);
    }
}