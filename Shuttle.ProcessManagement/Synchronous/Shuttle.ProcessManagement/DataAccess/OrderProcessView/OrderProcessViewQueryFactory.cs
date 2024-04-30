using System;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.DataAccess.OrderProcessView
{
    public class OrderProcessViewQueryFactory : IOrderProcessViewQueryFactory
    {
        public IQuery All()
        {
            return new Query(string.Concat(SelectClause(), "order by OrderDate"));
        }

        public IQuery Add(OrderProcessRegistered message)
        {
            return new Query(@"
insert into dbo.OrderProcessView
(
    Id,
    CustomerName,
    OrderNumber,
    OrderDate,
    OrderTotal,
    Status,
    TargetSystem,
    TargetSystemUri
)
values
(
    @Id,
    @CustomerName,
    @OrderNumber,
    @OrderDate,
    @OrderTotal,
    @Status,
    @TargetSystem,
    @TargetSystemUri
)
")
                .AddParameter(OrderProcessViewColumns.Id, message.OrderProcessId)
                .AddParameter(OrderProcessViewColumns.CustomerName, message.CustomerName)
                .AddParameter(OrderProcessViewColumns.OrderNumber, message.OrderNumber)
                .AddParameter(OrderProcessViewColumns.OrderDate, message.OrderDate)
                .AddParameter(OrderProcessViewColumns.OrderTotal, message.OrderTotal)
                .AddParameter(OrderProcessViewColumns.Status, message.Status)
                .AddParameter(OrderProcessViewColumns.TargetSystem, message.TargetSystem)
                .AddParameter(OrderProcessViewColumns.TargetSystemUri, message.TargetSystemUri);
        }

        public IQuery Find(Guid id)
        {
            return new Query(string.Concat(SelectClause(), "where Id = @Id"))
                .AddParameter(OrderProcessViewColumns.Id, id);
        }

        public IQuery Remove(Guid id)
        {
            return new Query("delete from dbo.OrderProcessView where Id = @Id")
                .AddParameter(OrderProcessViewColumns.Id, id);
        }

        public IQuery SaveStatus(Guid id, string status)
        {
            return new Query("update dbo.OrderProcessView set Status = @Status where Id = @Id")
                .AddParameter(OrderProcessViewColumns.Id, id)
                .AddParameter(OrderProcessViewColumns.Status, status);
        }

        private static string SelectClause()
        {
            return @"
select 
    Id, 
    CustomerName, 
    OrderNumber, 
    OrderDate, 
    OrderTotal, 
    Status,
    TargetSystem,
    TargetSystemUri
from 
    OrderProcessView ";
        }
    }
}