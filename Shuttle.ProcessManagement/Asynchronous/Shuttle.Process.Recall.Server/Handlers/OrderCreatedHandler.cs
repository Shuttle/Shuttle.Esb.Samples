using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Ordering.Messages;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class OrderCreatedHandler : IAsyncMessageHandler<OrderCreated>
{
    private readonly IEventStore _eventStore;

    public OrderCreatedHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<OrderCreated> context)
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

        stream.AddEvent(orderProcess.ChangeStatus("Order Created"));
        stream.AddEvent(orderProcess.AssignOrderId(context.Message.OrderId));

        await _eventStore.SaveAsync(stream);

        await context.SendAsync(orderProcess.CreateInvoiceCommand());
    }
}