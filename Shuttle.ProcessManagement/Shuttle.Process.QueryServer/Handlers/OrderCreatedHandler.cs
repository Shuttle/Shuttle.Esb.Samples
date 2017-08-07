using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.QueryServer
{
    public class OrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrderCreatedHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<OrderCreatedEvent> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _orderProcessViewQuery.SaveStatus(context.TransportMessage.OrderProcessId(), "Order Created");
            }
        }
    }
}