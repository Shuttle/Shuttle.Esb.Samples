﻿using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Ordering.Messages;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class OrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public OrderCreatedHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<OrderCreatedEvent> context)
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
                        string.Format("Could not find an order process with correlation id '{0}'.",
                            context.TransportMessage.CorrelationId));
                }

                stream.Apply(orderProcess);

                stream.AddEvent(orderProcess.ChangeStatus("Order Created"));
                stream.AddEvent(orderProcess.AssignOrderId(context.Message.OrderId));

                _eventStore.Save(stream);
            }

            context.Send(orderProcess.CreateInvoiceCommand());
        }
    }
}