using System;
using Shuttle.Core.Data;
using Shuttle.Process.Custom.Server.Domain;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessQueryFactory : IOrderProcessQueryFactory
    {
        public IQuery Add(OrderProcess orderProcess)
        {
            return new Query(@"
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
                .AddParameter(OrderProcessColumns.Id, orderProcess.Id)
                .AddParameter(OrderProcessColumns.OrderId, (object) orderProcess.OrderId ?? DBNull.Value)
                .AddParameter(OrderProcessColumns.InvoiceId, (object) orderProcess.InvoiceId ?? DBNull.Value)
                .AddParameter(OrderProcessColumns.CustomerName, orderProcess.CustomerName)
                .AddParameter(OrderProcessColumns.CustomerEMail, orderProcess.CustomerEMail)
                .AddParameter(OrderProcessColumns.DateRegistered, orderProcess.DateRegistered)
                .AddParameter(OrderProcessColumns.OrderNumber, orderProcess.OrderNumber)
                .AddParameter(OrderProcessColumns.TargetSystem, orderProcess.TargetSystem)
                .AddParameter(OrderProcessColumns.TargetSystemUri, orderProcess.TargetSystemUri);
        }

        public IQuery AddItem(OrderProcessItem orderProcessItem, Guid id)
        {
            return new Query(@"
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
                .AddParameter(OrderProcessItemColumns.OrderProcessId, id)
                .AddParameter(OrderProcessItemColumns.ProductId, orderProcessItem.ProductId)
                .AddParameter(OrderProcessItemColumns.Description, orderProcessItem.Description)
                .AddParameter(OrderProcessItemColumns.Price, orderProcessItem.Price);
        }

        public IQuery AddStatus(OrderProcessStatus status, Guid id)
        {
            return new Query(@"
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
                .AddParameter(OrderProcessStatusColumns.OrderProcessId, id)
                .AddParameter(OrderProcessStatusColumns.Status, status.Status)
                .AddParameter(OrderProcessStatusColumns.StatusDate, status.StatusDate);
        }

        public IQuery Get(Guid id)
        {
            return new Query(@"
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
                .AddParameter(OrderProcessColumns.Id, id);
        }

        public IQuery GetItems(Guid id)
        {
            return new Query(@"
select
    ProductId,
    Description,
    Price
from 
    dbo.OrderProcessItem
where
    OrderProcessId = @OrderProcessId
")
                .AddParameter(OrderProcessItemColumns.OrderProcessId, id);
        }

        public IQuery GetStatuses(Guid id)
        {
            return new Query(@"
select
    Status,
    StatusDate
from 
    dbo.OrderProcessStatus
where
    OrderProcessId = @OrderProcessId
")
                .AddParameter(OrderProcessStatusColumns.OrderProcessId, id);
        }

        public IQuery Remove(Guid id)
        {
            return
                new Query(@"delete from dbo.OrderProcess where Id = @Id")
                    .AddParameter(OrderProcessColumns.Id, id);
        }

        public IQuery SaveOrderId(Guid orderId, Guid id)
        {
            return new Query(@"update dbo.OrderProcess set OrderId = @OrderId where Id = @Id")
                .AddParameter(OrderProcessColumns.OrderId, orderId)
                .AddParameter(OrderProcessColumns.Id, id);
        }

        public IQuery SaveInvoiceId(Guid invoiceId, Guid id)
        {
            return new Query(@"update dbo.OrderProcess set invoiceId = @invoiceId where Id = @Id")
                .AddParameter(OrderProcessColumns.InvoiceId, invoiceId)
                .AddParameter(OrderProcessColumns.Id, id);
        }
    }
}