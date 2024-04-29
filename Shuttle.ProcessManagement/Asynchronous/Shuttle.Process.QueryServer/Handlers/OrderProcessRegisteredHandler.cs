using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.DataAccess.OrderProcessView;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer
{
    public class OrderProcessRegisteredHandler : IAsyncMessageHandler<OrderProcessRegistered>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrderProcessRegisteredHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            _orderProcessViewQuery = Guard.AgainstNull(orderProcessViewQuery, nameof(orderProcessViewQuery));
        }

        public async Task ProcessMessageAsync(IHandlerContext<OrderProcessRegistered> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                await _orderProcessViewQuery.AddAsync(context.Message);
            }
        }
    }
}