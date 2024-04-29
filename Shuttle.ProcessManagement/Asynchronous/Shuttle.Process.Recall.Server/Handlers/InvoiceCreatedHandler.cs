using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class InvoiceCreatedHandler : IAsyncMessageHandler<InvoiceCreated>
{
    private readonly IEventStore _eventStore;

    public InvoiceCreatedHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<InvoiceCreated> context)
    {
        if (!context.TransportMessage.IsHandledHere())
        {
            return;
        }

        var orderProcessId = new Guid(context.TransportMessage.CorrelationId);

        var orderProcess = new OrderProcess(orderProcessId);

        var stream = await _eventStore.GetAsync(orderProcessId);

        if (stream.IsEmpty)
        {
            throw new ApplicationException(
                $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
        }

        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Invoice Created"));
        stream.AddEvent(orderProcess.AssignInvoiceId(context.Message.InvoiceId));

        await _eventStore.SaveAsync(stream);

        await context.SendAsync(orderProcess.SendEMailCommand());
    }
}