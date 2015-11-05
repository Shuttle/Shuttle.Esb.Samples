using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.HandRolled.Server
{
    public class AcceptOrderProcessHandler : IMessageHandler<AcceptOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public AcceptOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(repository, "repository");

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(HandlerContext<AcceptOrderProcessCommand> context)
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

            var createOrderCommand = new CreateOrderCommand
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

            context.Publish(new OrderProcessAcceptedEvent
            {
                OrderProcessId = orderProcess.Id
            });
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}