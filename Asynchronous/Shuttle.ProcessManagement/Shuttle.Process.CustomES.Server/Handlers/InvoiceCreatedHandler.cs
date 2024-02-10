using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class InvoiceCreatedHandler : IMessageHandler<InvoiceCreated>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public InvoiceCreatedHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public void ProcessMessage(IHandlerContext<InvoiceCreated> context)
        {
            if (!context.TransportMessage.IsHandledHere())
            {
                return;
            }

            var orderProcessId = new Guid(context.TransportMessage.CorrelationId);

            var orderProcess = new OrderProcess(orderProcessId);

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var stream = _eventStore.Get(orderProcessId);

                if (stream.IsEmpty)
                {
                    throw new ApplicationException(
                        $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
                }

                stream.Apply(orderProcess);

                stream.AddEvent(orderProcess.ChangeStatus("Invoice Created"));
                stream.AddEvent(orderProcess.AssignInvoiceId(context.Message.InvoiceId));

                _eventStore.Save(stream);
            }

            context.Send(orderProcess.SendEMailCommand());
        }
    }
}