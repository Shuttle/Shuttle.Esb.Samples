using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class CompleteOrderProcessHandler : IAsyncMessageHandler<CompleteOrderProcess>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public CompleteOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public async Task ProcessMessageAsync(IHandlerContext<CompleteOrderProcess> context)
        {
            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = await _repository.GetAsync(context.Message.OrderProcessId);

                var orderProcessStatus = new OrderProcessStatus("Completed");

                orderProcess.AddStatus(orderProcessStatus);

                await _repository.AddStatusAsync(orderProcessStatus, orderProcess.Id);
            }

            await context.PublishAsync(new OrderProcessCompleted
            {
                OrderProcessId = orderProcess.Id
            });
        }
    }
}