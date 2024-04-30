using System;
using Shuttle.Core.Data;
using Shuttle.Ordering.Domain;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderQueryFactory : IOrderQueryFactory
    {
        public IQuery Add(Order order)
        {
            return new Query(@"
insert into dbo.[Order]
(
    Id,
    OrderNumber,
    OrderDate,
    CustomerName,
    CustomerEMail
)
values
(
    @Id,
    @OrderNumber,
    @OrderDate,
    @CustomerName,
    @CustomerEMail
)
")
                .AddParameter(OrderColumns.Id, order.Id)
                .AddParameter(OrderColumns.OrderNumber, order.OrderNumber)
                .AddParameter(OrderColumns.OrderDate, order.OrderDate)
                .AddParameter(OrderColumns.CustomerName, order.Customer.Name)
                .AddParameter(OrderColumns.CustomerEMail, order.Customer.EMail);
        }

        public IQuery AddItem(OrderItem orderItem, Guid orderId)
        {
            return new Query(@"
insert into dbo.OrderItem
(
    OrderId,
    Description,
    Price
)
values
(
    @OrderId,
    @Description,
    @Price
)
")
                .AddParameter(OrderItemColumns.OrderId, orderId)
                .AddParameter(OrderItemColumns.Description, orderItem.Description)
                .AddParameter(OrderItemColumns.Price, orderItem.Price);
        }
    }
}