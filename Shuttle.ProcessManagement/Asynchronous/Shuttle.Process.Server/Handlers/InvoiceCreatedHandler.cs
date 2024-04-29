using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;

namespace Shuttle.Process.Custom.Server
{
    public class InvoiceCreatedHandler : IAsyncMessageHandler<InvoiceCreated>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public InvoiceCreatedHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public async Task ProcessMessageAsync(IHandlerContext<InvoiceCreated> context)
        {
            if (!context.TransportMessage.IsHandledHere())
            {
                return;
            }

            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = await _repository.GetAsync(new Guid(context.TransportMessage.CorrelationId));

                if (orderProcess == null)
                {
                    throw new ApplicationException($"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
                }

                var orderProcessStatus = new OrderProcessStatus("Invoice Created");

                orderProcess.AddStatus(orderProcessStatus);

                await _repository.SaveInvoiceIdAsync(context.Message.InvoiceId, orderProcess.Id);
                await _repository.AddStatusAsync(orderProcessStatus, orderProcess.Id);
            }

            await context.SendAsync(new SendEMail
            {
                RecipientEMail = orderProcess.CustomerEMail,
                HtmlBody = $"Hello {orderProcess.CustomerName},<br/><br/>Your order number {orderProcess.OrderNumber} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team"
            });
        }
    }
}