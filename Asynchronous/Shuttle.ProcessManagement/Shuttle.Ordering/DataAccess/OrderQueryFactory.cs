using System;
using Shuttle.Core.Data;
using Shuttle.Ordering.Domain;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderQueryFactory : IOrderQueryFactory
    {
        public IQuery Add(Order order)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(OrderColumns.Id, order.Id)
                .AddParameterValue(OrderColumns.OrderNumber, order.OrderNumber)
                .AddParameterValue(OrderColumns.OrderDate, order.OrderDate)
                .AddParameterValue(OrderColumns.CustomerName, order.Customer.Name)
                .AddParameterValue(OrderColumns.CustomerEMail, order.Customer.EMail);
        }

        public IQuery AddItem(OrderItem orderItem, Guid orderId)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(OrderItemColumns.OrderId, orderId)
                .AddParameterValue(OrderItemColumns.Description, orderItem.Description)
                .AddParameterValue(OrderItemColumns.Price, orderItem.Price);
        }
    }
}