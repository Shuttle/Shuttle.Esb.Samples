using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Invoicing.Domain;
using Shuttle.Invoicing.Messages;

namespace Shuttle.Invoicing.Server
{
    public class CreateInvoiceHandler : IAsyncMessageHandler<CreateInvoice>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IInvoiceRepository _repository;

        public CreateInvoiceHandler(IDatabaseContextFactory databaseContextFactory, IInvoiceRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public async Task ProcessMessageAsync(IHandlerContext<CreateInvoice> context)
        {
            // simulate slow processing
            await Task.Delay(2000);

            var message = context.Message;

            var invoice = new Invoice(message.OrderId)
            {
                AccountContact = new InvoiceAccountContact(message.AccountContactName, message.AccountContactEMail)
            };

            foreach (var item in message.Items)
            {
                invoice.AddItem(item.Description, item.Price);
            }

            invoice.GenerateInvoiceNumber();

            using (_databaseContextFactory.Create())
            {
                await _repository.AddAsync(invoice);
            }

            var orderCreatedEvent = new InvoiceCreated
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                AccountContactName = message.AccountContactName,
                AccountContactEMail = message.AccountContactEMail
            };

            orderCreatedEvent.Items.AddRange(message.Items);

            await context.PublishAsync(orderCreatedEvent);
        }
    }
}