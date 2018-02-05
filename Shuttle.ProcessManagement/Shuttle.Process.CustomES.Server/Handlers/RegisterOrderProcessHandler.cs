﻿using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class RegisterOrderProcessHandler : IMessageHandler<RegisterOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public RegisterOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(eventStore, nameof(eventStore));

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<RegisterOrderProcessCommand> context)
        {
            var message = context.Message;

            var orderProcess = new OrderProcess();

            var stream = _eventStore.CreateEventStream(orderProcess.Id);

            var initialized = orderProcess.Initialize();

            stream.AddEvent(initialized);
            stream.AddEvent(orderProcess.AssignCustomer(message.CustomerName, message.CustomerEMail));
            stream.AddEvent(orderProcess.AssignTargetSystem(message.TargetSystem, message.TargetSystemUri));

            var status = orderProcess.ChangeStatus("Cooling Off");

            stream.AddEvent(status);

            foreach (var quotedProduct in message.QuotedProducts)
            {
                stream.AddEvent(orderProcess.AddItem(quotedProduct.ProductId, quotedProduct.Description,
                    quotedProduct.Price));
            }

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _eventStore.Save(stream);
            }

            context.Publish(new OrderProcessRegisteredEvent
            {
                OrderProcessId = orderProcess.Id,
                QuotedProducts = message.QuotedProducts,
                CustomerName = message.CustomerName,
                CustomerEMail = message.CustomerEMail,
                OrderNumber = initialized.OrderNumber,
                OrderDate = initialized.DateRegistered,
                OrderTotal = orderProcess.Total,
                Status = status.Status,
                StatusDate = status.StatusDate,
                TargetSystem = message.TargetSystem,
                TargetSystemUri = message.TargetSystemUri
            });

            context.Send(new AcceptOrderProcessCommand
            {
                OrderProcessId = orderProcess.Id
            }, c => c.Defer(DateTime.Now.AddSeconds(10)).Local());
        }
    }
}