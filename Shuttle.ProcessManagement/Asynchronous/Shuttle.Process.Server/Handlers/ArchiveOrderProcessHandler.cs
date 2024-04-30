using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class ArchiveOrderProcessHandler : IAsyncMessageHandler<ArchiveOrderProcess>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public ArchiveOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public async Task ProcessMessageAsync(IHandlerContext<ArchiveOrderProcess> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = await _repository.GetAsync(context.Message.OrderProcessId);

                if (orderProcess == null)
                {
                    return;
                }

                if (!orderProcess.CanArchive())
                {
                    await context.PublishAsync(new ArchiveOrderProcessRejected
                    {
                        OrderProcessId = context.Message.OrderProcessId,
                        Status = orderProcess.Status().Status
                    });

                    return;
                }

                await _repository.RemoveAsync(orderProcess);
            }

            await context.PublishAsync(new OrderProcessArchived
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}