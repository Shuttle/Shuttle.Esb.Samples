﻿using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class AcceptOrderProcessHandler : IMessageHandler<AcceptOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public AcceptOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<AcceptOrderProcessCommand> context)
        {
            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var stream = _eventStore.Get(context.Message.OrderProcessId);

                if (stream.IsEmpty)
                {
                    return;
                }

                orderProcess = new OrderProcess(context.Message.OrderProcessId);
                stream.Apply(orderProcess);

                stream.AddEvent(orderProcess.ChangeStatus("Order Accepted"));

                _eventStore.Save(stream);
            }

            context.Send(orderProcess.CreateOrderCommand(), c => c.WithCorrelationId(orderProcess.Id.ToString()));

            context.Publish(new OrderProcessAcceptedEvent
            {
                OrderProcessId = orderProcess.Id
            });
        }
    }
}