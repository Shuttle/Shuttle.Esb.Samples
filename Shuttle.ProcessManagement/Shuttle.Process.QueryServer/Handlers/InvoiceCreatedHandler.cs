using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.QueryServer
{
    public class InvoiceCreatedHandler : IMessageHandler<InvoiceCreatedEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public InvoiceCreatedHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<InvoiceCreatedEvent> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _orderProcessViewQuery.SaveStatus(context.TransportMessage.OrderProcessId(), "Invoice Created");
            }
        }
    }
}