using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.DataAccess.OrderProcessView;

namespace Shuttle.Process.QueryServer
{
    public class EMailSentHandler : IAsyncMessageHandler<EMailSent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public EMailSentHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            _orderProcessViewQuery = Guard.AgainstNull(orderProcessViewQuery, nameof(orderProcessViewQuery));
        }

        public async Task ProcessMessageAsync(IHandlerContext<EMailSent> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                await _orderProcessViewQuery.SaveStatusAsync(context.TransportMessage.OrderProcessId(), "E-Mail Sent");
            }
        }
    }
}