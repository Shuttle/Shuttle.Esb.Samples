using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.DataAccess.OrderProcessView;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer
{
    public class OrderProcessRegisteredHandler : IMessageHandler<OrderProcessRegistered>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrderProcessRegisteredHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(orderProcessViewQuery, nameof(orderProcessViewQuery));

            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
        }

        public void ProcessMessage(IHandlerContext<OrderProcessRegistered> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _orderProcessViewQuery.Add(context.Message);
            }
        }
    }
}