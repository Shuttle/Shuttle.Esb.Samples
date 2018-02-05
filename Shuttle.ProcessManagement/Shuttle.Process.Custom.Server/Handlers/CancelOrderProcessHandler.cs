using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class CancelOrderProcessHandler : IMessageHandler<CancelOrderProcessCommand>
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

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<CancelOrderProcessCommand> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = _repository.Get(context.Message.OrderProcessId);

                if (!orderProcess.CanCancel())
                {
                    return;
                }

                _repository.Remove(orderProcess);
            }

            context.Publish(new OrderProcessCancelledEvent
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}