using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.DataAccess.OrderProcessView;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer
{
    public class ArchiveOrderProcessRejectedHandler : IAsyncMessageHandler<ArchiveOrderProcessRejected>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public ArchiveOrderProcessRejectedHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            _databaseContextFactory = Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            _orderProcessViewQuery = Guard.AgainstNull(orderProcessViewQuery, nameof(orderProcessViewQuery));
        }

        public async Task ProcessMessageAsync(IHandlerContext<ArchiveOrderProcessRejected> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                await _orderProcessViewQuery.SaveStatusAsync(context.Message.OrderProcessId, context.Message.Status);
            }
        }
    }
}