using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.ESB.Process;
using Shuttle.Process.ESModule.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall.Core;

namespace Shuttle.Process.ESModule.Server
{
    public class RegisterOrderProcessHandler : IMessageHandler<RegisterOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public RegisterOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public void ProcessMessage(HandlerContext<RegisterOrderProcessCommand> context)
        {
            var message = context.Message;

            var orderProcess = new OrderProcess();

            var stream = new EventStream(orderProcess.CorrelationId);

            var initialized = orderProcess.Initialize();

            stream.AddEvent(initialized);
            stream.AddEvent(orderProcess.AssignCustomer(message.CustomerName, message.CustomerEMail));
            stream.AddEvent(orderProcess.AssignTargetSystem(message.TargetSystem, message.TargetSystemUri));
            
            var status = orderProcess.ChangeStatus("Cooling Off");
            
            stream.AddEvent(status);

            foreach (var quotedProduct in message.QuotedProducts)
            {
                stream.AddEvent(orderProcess.AddItem(quotedProduct.ProductId, quotedProduct.Description, quotedProduct.Price));
            }

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _eventStore.SaveEventStream(stream);
            }

            context.Publish(new OrderProcessRegisteredEvent
            {
                OrderProcessId = orderProcess.CorrelationId,
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
                OrderProcessId = orderProcess.CorrelationId
            }, c => c.Defer(DateTime.Now.AddSeconds(10)).Local().WithProcess(orderProcess));
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}