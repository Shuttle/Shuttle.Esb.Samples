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

        public IQuery Get(Guid id)
        {
            return RawQuery.Create(@"
select
    Id,
    OrderId,
    InvoiceId,
    CustomerName,
    CustomerEMail
from 
    dbo.OrderProcess
where
    Id = @Id
")
                .AddParameterValue(OrderProcessColumns.Id, id);
        }

        public IQuery GetItems(Guid id)
        {
            return RawQuery.Create(@"
select
    ProductId,
    Description,
    Price
from 
    dbo.OrderProcessItem
where
    OrderProcessId = @OrderProcessId
")
                .AddParameterValue(OrderProcessItemColumns.OrderProcessId, id);
        }

        public IQuery GetStatuses(Guid id)
        {
            return RawQuery.Create(@"
select
    Status,
    StatusDate
from 
    dbo.OrderProcessStatus
where
    OrderProcessId = @OrderProcessId
")
                .AddParameterValue(OrderProcessStatusColumns.OrderProcessId, id);
        }

        public IQuery Remove(Guid id)
        {
            return
                RawQuery.Create(@"delete from dbo.OrderProcess where Id = @Id")
                    .AddParameterValue(OrderProcessColumns.Id, id);
        }
    }
}