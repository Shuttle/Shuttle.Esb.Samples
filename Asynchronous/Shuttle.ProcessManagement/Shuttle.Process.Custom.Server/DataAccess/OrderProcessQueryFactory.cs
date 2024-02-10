using System;
using Shuttle.Core.Data;
using Shuttle.Process.Custom.Server.Domain;

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
    CustomerEMail,
    DateRegistered,
    OrderNumber,
    TargetSystem,
    TargetSystemUri
)
values
(
    @Id,
    @OrderId,
    @InvoiceId,
    @CustomerName,
    @CustomerEMail,
    @DateRegistered,
    @OrderNumber,
    @TargetSystem,
    @TargetSystemUri
)
")
                .AddParameterValue(OrderProcessColumns.Id, orderProcess.Id)
                .AddParameterValue(OrderProcessColumns.OrderId, (object) orderProcess.OrderId ?? DBNull.Value)
                .AddParameterValue(OrderProcessColumns.InvoiceId, (object) orderProcess.InvoiceId ?? DBNull.Value)
                .AddParameterValue(OrderProcessColumns.CustomerName, orderProcess.CustomerName)
                .AddParameterValue(OrderProcessColumns.CustomerEMail, orderProcess.CustomerEMail)
                .AddParameterValue(OrderProcessColumns.DateRegistered, orderProcess.DateRegistered)
                .AddParameterValue(OrderProcessColumns.OrderNumber, orderProcess.OrderNumber)
                .AddParameterValue(OrderProcessColumns.TargetSystem, orderProcess.TargetSystem)
                .AddParameterValue(OrderProcessColumns.TargetSystemUri, orderProcess.TargetSystemUri);
        }

        public IQuery AddItem(OrderProcessItem orderProcessItem, Guid id)
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
                .AddParameterValue(OrderProcessItemColumns.OrderProcessId, id)
                .AddParameterValue(OrderProcessItemColumns.ProductId, orderProcessItem.ProductId)
                .AddParameterValue(OrderProcessItemColumns.Description, orderProcessItem.Description)
                .AddParameterValue(OrderProcessItemColumns.Price, orderProcessItem.Price);
        }

        public IQuery AddStatus(OrderProcessStatus status, Guid id)
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
                .AddParameterValue(OrderProcessStatusColumns.OrderProcessId, id)
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
    CustomerEMail,
    DateRegistered,
    OrderNumber,
    TargetSystem,
    TargetSystemUri
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

        public IQuery SaveOrderId(Guid orderId, Guid id)
        {
            return RawQuery.Create(@"update dbo.OrderProcess set OrderId = @OrderId where Id = @Id")
                .AddParameterValue(OrderProcessColumns.OrderId, orderId)
                .AddParameterValue(OrderProcessColumns.Id, id);
        }

        public IQuery SaveInvoiceId(Guid invoiceId, Guid id)
        {
            return RawQuery.Create(@"update dbo.OrderProcess set invoiceId = @invoiceId where Id = @Id")
                .AddParameterValue(OrderProcessColumns.InvoiceId, invoiceId)
                .AddParameterValue(OrderProcessColumns.Id, id);
        }
    }
}