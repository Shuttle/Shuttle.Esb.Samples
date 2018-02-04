using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class EMailSentHandler : IMessageHandler<EMailSentEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public EMailSentHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<EMailSentEvent> context)
        {
            if (!context.TransportMessage.IsHandledHere())
            {
                return;
            }

            var orderProcessId = new Guid(context.TransportMessage.CorrelationId);

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var stream = _eventStore.Get(orderProcessId);

                if (stream.IsEmpty)
                {
                    throw new ApplicationException(
                        string.Format("Could not find an order process with correlation id '{0}'.",
                            context.TransportMessage.CorrelationId));
                }

                var orderProcess = new OrderProcess(orderProcessId);
                stream.Apply(orderProcess);

                stream.AddEvent(orderProcess.ChangeStatus("Dispatched-EMail Sent"));

                _eventStore.Save(stream);
            }

            context.Send(new CompleteOrderProcessCommand
            {
                OrderProcessId = orderProcessId
            }, c => c.Local());
        }
    }
}