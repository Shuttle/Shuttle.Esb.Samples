using System;
using Shuttle.Core.Data;
using Shuttle.Ordering.Domain;

namespace Shuttle.Ordering.DataAccess
{
    public interface IOrderQueryFactory
    {
        IQuery Add(Order order);
        IQuery AddItem(OrderItem orderItem, Guid orderId);
    }
}