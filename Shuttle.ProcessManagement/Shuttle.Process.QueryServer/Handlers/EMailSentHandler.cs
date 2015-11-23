using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.EMailSender.Messages;
using Shuttle.ESB.Core;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.QueryServer
{
    public class EMailSentHandler : IMessageHandler<EMailSentEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public EMailSentHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
        }

        public void ProcessMessage(IHandlerContext<EMailSentEvent> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _orderProcessViewQuery.SaveStatus(context.TransportMessage.OrderProcessId(), "Dispatched E-Mail Sent");
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}