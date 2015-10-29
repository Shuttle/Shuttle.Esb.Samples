using System;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessQueryFactory : IOrderProcessQueryFactory
    {
        public IQuery Add(OrderProcess orderProcess)
        {
            return RawQuery.Create(@"
insert into dbo.OrderProcess
(
    Id,
    OrderId,
    InvoiceId,
    CustomerName,
    CustomerEMail
)
values
(
    @Id,
    @OrderId,
    @InvoiceId,
    @CustomerName,
    @CustomerEMail
)
")
                .AddParameterValue(OrderProcessColumns.Id, orderProcess.Id)
                .AddParameterValue(OrderProcessColumns.OrderId, (object)orderProcess.OrderId ?? DBNull.Value)
                .AddParameterValue(OrderProcessColumns.InvoiceId, (object)orderProcess.InvoiceId ?? DBNull.Value)
                .AddParameterValue(OrderProcessColumns.CustomerName, orderProcess.CustomerName)
                .AddParameterValue(OrderProcessColumns.CustomerEMail, orderProcess.CustomerEMail);
        }

        public IQuery AddItem(OrderProcessItem orderProcessItem, Guid orderProcessId)
        {
            return RawQuery.Create(@"
insert into dbo.OrderProcessItem
(
    OrderProcessId,
    ProductId,
    Description,
    Price
)
values
(
    @OrderProcessId,
    @ProductId,
    @Description,
    @Price
)
")
                .AddParameterValue(OrderProcessItemColumns.OrderProcessId, orderProcessId)
                .AddParameterValue(OrderProcessItemColumns.ProductId, orderProcessItem.ProductId)
                .AddParameterValue(OrderProcessItemColumns.Description, orderProcessItem.Description)
                .AddParameterValue(OrderProcessItemColumns.Price, orderProcessItem.Price);
        }

        public IQuery AddStatus(OrderProcessStatus status, Guid orderProcessId)
        {
            return RawQuery.Create(@"
insert into dbo.OrderProcessStatus
(
    OrderProcessId,
    Status,
    StatusDate
)
values
(
    @OrderProcessId,
    @Status,
    @StatusDate
)
")
                .AddParameterValue(OrderProcessStatusColumns.OrderProcessId, orderProcessId)
                .AddParameterValue(OrderProcessStatusColumns.Status, status.Status)
                .AddParameterValue(OrderProcessStatusColumns.StatusDate, status.StatusDate);
        }
    }
}