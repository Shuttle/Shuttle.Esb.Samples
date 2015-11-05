using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Ordering.Domain;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderQueryFactory _queryFactory;

        public OrderRepository(IDatabaseGateway databaseGateway, IOrderQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");
            Guard.AgainstNull(queryFactory, "queryFactory");

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public void Add(Order order)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Add(order));

            foreach (var item in order.Items)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.AddItem(item, order.Id));
            }
        }
    }
}