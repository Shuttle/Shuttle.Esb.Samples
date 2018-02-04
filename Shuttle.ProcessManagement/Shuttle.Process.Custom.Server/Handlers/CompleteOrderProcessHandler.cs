using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class CompleteOrderProcessHandler : IMessageHandler<CompleteOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public CompleteOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(repository, "repository");

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<CompleteOrderProcessCommand> context)
        {
            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = _repository.Get(context.Message.OrderProcessId);

                var orderProcessStatus = new OrderProcessStatus("Completed");

                orderProcess.AddStatus(orderProcessStatus);

                _repository.AddStatus(orderProcessStatus, orderProcess.Id);
            }

            context.Publish(new OrderProcessCompletedEvent
            {
                OrderProcessId = orderProcess.Id
            });
        }
    }
}