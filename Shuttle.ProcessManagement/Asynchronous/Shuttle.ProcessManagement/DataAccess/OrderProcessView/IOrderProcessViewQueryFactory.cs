using System;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.DataAccess.OrderProcessView
{
    public interface IOrderProcessViewQueryFactory
    {
        IQuery All();
        IQuery Add(OrderProcessRegistered message);
        IQuery Find(Guid id);
        IQuery Remove(Guid id);
        IQuery SaveStatus(Guid id, string status);
    }
}