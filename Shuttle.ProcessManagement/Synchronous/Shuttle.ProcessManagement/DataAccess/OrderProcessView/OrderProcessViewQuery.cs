using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.DataAccess.OrderProcessView
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
            return _databaseGateway.GetRows(_queryFactory.All());
        }

        public void Add(OrderProcessRegistered message)
        {
            _databaseGateway.Execute(_queryFactory.Add(message));
        }

        public DataRow Find(Guid id)
        {
            return _databaseGateway.GetRow(_queryFactory.Find(id));
        }

        public void Remove(Guid id)
        {
            _databaseGateway.Execute(_queryFactory.Remove(id));
        }

        public void SaveStatus(Guid orderProcessId, string status)
        {
            _databaseGateway.Execute(_queryFactory.SaveStatus(orderProcessId, status));
        }
    }
}