using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.Custom.Server
{
    public class InvoiceCreatedHandler : IMessageHandler<InvoiceCreated>
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

        public void ProcessMessage(IHandlerContext<InvoiceCreated> context)
        {
            if (!context.TransportMessage.IsHandledHere())
            {
                return;
            }

            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = _repository.Get(new Guid(context.TransportMessage.CorrelationId));

                if (orderProcess == null)
                {
                    throw new ApplicationException(
                        $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
                }

                var orderProcessStatus = new OrderProcessStatus("Invoice Created");

                orderProcess.AddStatus(orderProcessStatus);

                _repository.SaveInvoiceId(context.Message.InvoiceId, orderProcess.Id);
                _repository.AddStatus(orderProcessStatus, orderProcess.Id);
            }

            context.Send(new SendEMail
            {
                RecipientEMail = orderProcess.CustomerEMail,
                HtmlBody =
                    string.Format(
                        "Hello {0},<br/><br/>Your order number {1} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team",
                        orderProcess.CustomerName, orderProcess.OrderNumber)
            });
        }
    }
}