using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Ordering.Messages;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class AcceptOrderProcessHandler : IMessageHandler<AcceptOrderProcess>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public AcceptOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(IHandlerContext<AcceptOrderProcess> context)
        {
            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = _repository.Get(context.Message.OrderProcessId);

                if (orderProcess == null)
                {
                    // Probably cancelled in the meantime.
                    // In a production system you would probably check this against some audit mechanism.
                    return;
                }

                var orderProcessStatus = new OrderProcessStatus("Order Accepted");

                orderProcess.AddStatus(orderProcessStatus);

                _repository.AddStatus(orderProcessStatus, orderProcess.Id);
            }

            var createOrderCommand = new CreateOrder
            {
                OrderNumber = orderProcess.OrderNumber,
                OrderDate = orderProcess.DateRegistered,
                CustomerName = orderProcess.CustomerName,
                CustomerEMail = orderProcess.CustomerEMail
            };

            foreach (var orderProcessItem in orderProcess.OrderProcessItems)
            {
                createOrderCommand.Items.Add(new MessageOrderItem
                {
                    Description = orderProcessItem.Description,
                    Price = orderProcessItem.Price
                });
            }

            context.Send(createOrderCommand, c => c.WithCorrelationId(orderProcess.Id.ToString()));

            context.Publish(new OrderProcessAccepted
            {
                OrderProcessId = orderProcess.Id
            });
        }
    }
}