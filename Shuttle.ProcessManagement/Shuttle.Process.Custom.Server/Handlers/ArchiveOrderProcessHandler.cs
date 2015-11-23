using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class ArchiveOrderProcessHandler : IMessageHandler<ArchiveOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public ArchiveOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(repository, "repository");

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(IHandlerContext<ArchiveOrderProcessCommand> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = _repository.Get(context.Message.OrderProcessId);

                if (!orderProcess.CanArchive())
                {
                    context.Publish(new ArchiveOrderProcessRejectedEvent
                    {
                        OrderProcessId = context.Message.OrderProcessId,
                        Status = orderProcess.Status().Status
                    });

                    return;
                }

                _repository.Remove(orderProcess);
            }

            context.Publish(new OrderProcessArchivedEvent
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }

        public bool IsReusable {
            get { return true; }
        }
    }
}