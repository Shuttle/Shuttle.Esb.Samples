﻿using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessViewQuery : IOrderProcessViewQuery
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderProcessViewQueryFactory _queryFactory;

        public OrderProcessViewQuery(IDatabaseGateway databaseGateway, IOrderProcessViewQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public IEnumerable<DataRow> All()
        {
            return _databaseGateway.GetRowsUsing(_queryFactory.All());
        }

        public void Add(OrderProcessRegisteredEvent message)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Add(message));
        }

        public DataRow Find(Guid id)
        {
            return _databaseGateway.GetSingleRowUsing(_queryFactory.Find(id));
        }

        public void Remove(Guid id)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Remove(id));
        }

        public void SaveStatus(Guid orderProcessId, string status)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.SaveStatus(orderProcessId, status));
        }
    }
}