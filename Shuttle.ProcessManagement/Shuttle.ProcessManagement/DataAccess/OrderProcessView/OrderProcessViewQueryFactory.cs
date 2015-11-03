﻿using System;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessViewQueryFactory : IOrderProcessViewQueryFactory
    {
        public IQuery All()
        {
            return RawQuery.Create(string.Concat(SelectClause(), "order by OrderDate"));
        }

        private static string SelectClause()
        {
            return @"
select 
    Id, 
    CustomerName, 
    OrderNumber, 
    OrderDate, 
    Status,
    TargetSystem,
    TargetSystemUri
from 
    OrderProcessView ";
        }

        public IQuery Add(OrderProcessRegisteredEvent message)
        {
            return RawQuery.Create(@"
insert into dbo.OrderProcessView
(
    Id,
    CustomerName,
    Status,
    TargetSystem,
    TargetSystemUri
)
values
(
    @Id,
    @CustomerName,
    @Status,
    @TargetSystem,
    @TargetSystemUri
)
")
                .AddParameterValue(OrderProcessViewColumns.Id, message.OrderProcessId)
                .AddParameterValue(OrderProcessViewColumns.CustomerName, message.CustomerName)
                .AddParameterValue(OrderProcessViewColumns.Status, message.Status)
                .AddParameterValue(OrderProcessViewColumns.TargetSystem, message.TargetSystem)
                .AddParameterValue(OrderProcessViewColumns.TargetSystemUri, message.TargetSystemUri);
        }

        public IQuery Find(Guid id)
        {
            return RawQuery.Create(string.Concat(SelectClause(), "where Id = @Id")).AddParameterValue(OrderProcessViewColumns.Id, id);
        }

        public IQuery Cancelling(Guid id)
        {
            return RawQuery.Create("update dbo.OrderProcessView set Status = 'Cancelling' where Id = @Id").AddParameterValue(OrderProcessViewColumns.Id, id);
        }

        public IQuery Remove(Guid id)
        {
            return RawQuery.Create("delete from dbo.OrderProcessView where Id = @Id").AddParameterValue(OrderProcessViewColumns.Id, id);
        }
    }
}