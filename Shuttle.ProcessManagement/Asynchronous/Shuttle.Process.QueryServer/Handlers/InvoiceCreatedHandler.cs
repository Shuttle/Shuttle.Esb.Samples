using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.DataAccess.OrderProcessView;

namespace Shuttle.Process.QueryServer
{
    public class InvoiceCreatedHandler : IAsyncMessageHandler<InvoiceCreated>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public InvoiceCreatedHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            _orderProcessViewQuery = Guard.AgainstNull(orderProcessViewQuery, nameof(orderProcessViewQuery));
        }

        public async Task ProcessMessageAsync(IHandlerContext<InvoiceCreated> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                await _orderProcessViewQuery.SaveStatusAsync(context.TransportMessage.OrderProcessId(), "Invoice Created");
            }
        }
    }
}