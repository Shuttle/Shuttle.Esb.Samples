using System.Threading;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Ordering.Domain;
using Shuttle.Ordering.Messages;

namespace Shuttle.Ordering.Server
{
    public class CreateOrderHandler : IMessageHandler<CreateOrderCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderRepository _repository;

        public CreateOrderHandler(IDatabaseContextFactory databaseContextFactory, IOrderRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(repository, "repository");

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(IHandlerContext<CreateOrderCommand> context)
        {
            // simulate slow processing
            Thread.Sleep(1000);

            var message = context.Message;

            var order = new Order(message.OrderNumber, message.OrderDate)
            {
                Customer = new OrderCustomer(message.CustomerName, message.CustomerEMail)
            };

            foreach (var item in message.Items)
            {
                order.AddItem(item.Description, item.Price);
            }

            using(_databaseContextFactory.Create(OrderingData.ConnectionStringName))
            {
                _repository.Add(order);
            }

            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                OrderNumber = message.OrderNumber,
                OrderDate = message.OrderDate,
                CustomerName = message.CustomerName,
                CustomerEMail = message.CustomerEMail
            };

            orderCreatedEvent.Items.AddRange(message.Items);

            context.Publish(orderCreatedEvent);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}