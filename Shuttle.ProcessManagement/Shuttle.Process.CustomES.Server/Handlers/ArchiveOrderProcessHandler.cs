﻿using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class ArchiveOrderProcessHandler : IMessageHandler<ArchiveOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public ArchiveOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<ArchiveOrderProcessCommand> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var stream = _eventStore.Get(context.Message.OrderProcessId);
                var orderProcess = new OrderProcess(context.Message.OrderProcessId);
                stream.Apply(orderProcess);

                if (!orderProcess.CanArchive)
                {
                    context.Publish(new ArchiveOrderProcessRejectedEvent
                    {
                        OrderProcessId = context.Message.OrderProcessId,
                        Status = orderProcess.Status
                    });

                    return;
                }

                stream.AddEvent(orderProcess.ChangeStatus("Order Archived"));

                _eventStore.Save(stream);
            }

            context.Publish(new OrderProcessArchivedEvent
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}