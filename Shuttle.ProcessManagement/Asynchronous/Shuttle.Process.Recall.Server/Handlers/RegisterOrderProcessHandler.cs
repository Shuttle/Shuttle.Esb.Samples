using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers
{
    public class RegisterOrderProcessHandler : IAsyncMessageHandler<RegisterOrderProcess>
    {
        private readonly IEventStore _eventStore;

        public RegisterOrderProcessHandler(IEventStore eventStore)
        {
            _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
        }

        public async Task ProcessMessageAsync(IHandlerContext<RegisterOrderProcess> context)
        {
            var message = context.Message;

            var orderProcess = new OrderProcess();

            var stream = await _eventStore.GetAsync(orderProcess.Id);

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

            await _eventStore.SaveAsync(stream);

            await context.PublishAsync(new OrderProcessRegistered
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

            await context.SendAsync(new AcceptOrderProcess
            {
                OrderProcessId = orderProcess.Id
            }, c => c.Defer(DateTime.Now.AddSeconds(10)).Local());
        }
    }
}