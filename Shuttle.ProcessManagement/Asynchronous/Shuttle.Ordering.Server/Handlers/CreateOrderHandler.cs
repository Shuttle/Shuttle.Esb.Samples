using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Ordering.Domain;
using Shuttle.Ordering.Messages;

namespace Shuttle.Ordering.Server
{
    public class CreateOrderHandler : IAsyncMessageHandler<CreateOrder>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderRepository _repository;

        public CreateOrderHandler(IDatabaseContextFactory databaseContextFactory, IOrderRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public async Task ProcessMessageAsync(IHandlerContext<CreateOrder> context)
        {
            // simulate slow processing
            await Task.Delay(1000);

            var message = context.Message;

            var order = new Order(message.OrderNumber, message.OrderDate)
            {
                Customer = new OrderCustomer(message.CustomerName, message.CustomerEMail)
            };

            foreach (var item in message.Items)
            {
                order.AddItem(item.Description, item.Price);
            }

            using (_databaseContextFactory.Create())
            {
                await _repository.AddAsync(order);
            }

            var orderCreatedEvent = new OrderCreated
            {
                OrderId = order.Id,
                OrderNumber = message.OrderNumber,
                OrderDate = message.OrderDate,
                CustomerName = message.CustomerName,
                CustomerEMail = message.CustomerEMail
            };

            orderCreatedEvent.Items.AddRange(message.Items);

            await context.PublishAsync(orderCreatedEvent);
        }
    }
}