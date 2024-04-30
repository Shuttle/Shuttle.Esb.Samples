using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class ArchiveOrderProcessHandler : IMessageHandler<ArchiveOrderProcess>
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

        public void ProcessMessage(IHandlerContext<ArchiveOrderProcess> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = _repository.Get(context.Message.OrderProcessId);

                if (!orderProcess.CanArchive())
                {
                    context.Publish(new ArchiveOrderProcessRejected
                    {
                        OrderProcessId = context.Message.OrderProcessId,
                        Status = orderProcess.Status().Status
                    });

                    return;
                }

                _repository.Remove(orderProcess);
            }

            context.Publish(new OrderProcessArchived
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}