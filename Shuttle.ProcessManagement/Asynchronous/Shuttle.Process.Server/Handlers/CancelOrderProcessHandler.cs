using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class CancelOrderProcessHandler : IAsyncMessageHandler<CancelOrderProcess>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public CancelOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public async Task ProcessMessageAsync(IHandlerContext<CancelOrderProcess> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = await _repository.GetAsync(context.Message.OrderProcessId);

                if (!orderProcess.CanCancel())
                {
                    return;
                }

                await _repository.RemoveAsync(orderProcess);
            }

            await context.PublishAsync(new OrderProcessCancelled
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}