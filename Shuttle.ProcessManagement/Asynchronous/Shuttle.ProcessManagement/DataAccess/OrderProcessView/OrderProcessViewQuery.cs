using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
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
            _databaseGateway = Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            _queryFactory = Guard.AgainstNull(queryFactory, nameof(queryFactory));
        }

        public async Task<IEnumerable<DataRow>> AllAsync()
        {
            return await _databaseGateway.GetRowsAsync(_queryFactory.All());
        }

        public async Task AddAsync(OrderProcessRegistered message)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.Add(message));
        }

        public async Task<DataRow> FindAsync(Guid id)
        {
            return await _databaseGateway.GetRowAsync(_queryFactory.Find(id));
        }

        public async Task RemoveAsync(Guid id)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.Remove(id));
        }

        public async Task SaveStatusAsync(Guid orderProcessId, string status)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.SaveStatus(orderProcessId, status));
        }
    }
}