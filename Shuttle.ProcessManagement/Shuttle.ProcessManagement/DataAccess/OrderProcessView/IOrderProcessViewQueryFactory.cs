using System;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessViewQueryFactory
    {
        IQuery All();
        IQuery Add(OrderProcessRegisteredEvent message);
        IQuery Find(Guid id);
        IQuery Cancelling(Guid id);
        IQuery Remove(Guid id);
    }
}