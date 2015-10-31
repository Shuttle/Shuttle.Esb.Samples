using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessViewQuery : IOrderProcessViewQuery
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderProcessViewQueryFactory _queryFactory;

        public OrderProcessViewQuery(IDatabaseGateway databaseGateway, IOrderProcessViewQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");
            Guard.AgainstNull(queryFactory, "queryFactory");

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
    }
}