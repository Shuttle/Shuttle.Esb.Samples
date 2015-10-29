using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessRepository : IOrderProcessRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderProcessQueryFactory _queryFactory;

        public OrderProcessRepository( IDatabaseGateway databaseGateway, IOrderProcessQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public void Add(OrderProcess orderProcess)
        {
            Guard.AgainstNull(orderProcess, "orderProcess");

            _databaseGateway.ExecuteUsing(_queryFactory.Add(orderProcess));

            foreach (var orderProcessItem in orderProcess.OrderProcessItems)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.AddItem(orderProcessItem, orderProcess.Id));
            }

            foreach (var status in orderProcess.Statuses)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.AddStatus(status, orderProcess.Id));
            }
        }
    }
}