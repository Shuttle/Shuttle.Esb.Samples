using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Ordering.Domain;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderQueryFactory _queryFactory;

        public OrderRepository(IDatabaseGateway databaseGateway, IOrderQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public async Task AddAsync(Order order)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.Add(order));

            foreach (var item in order.Items)
            {
                await _databaseGateway.ExecuteAsync(_queryFactory.AddItem(item, order.Id));
            }
        }
    }
}