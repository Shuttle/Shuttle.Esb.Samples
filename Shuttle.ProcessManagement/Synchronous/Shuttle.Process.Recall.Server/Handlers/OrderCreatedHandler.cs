using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Ordering.Messages;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class OrderCreatedHandler : IMessageHandler<OrderCreated>
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

    public void ProcessMessage(IHandlerContext<OrderCreated> context)
    {
        if (!context.TransportMessage.IsHandledHere())
        {
            return;
        }

        var orderProcessId = new Guid(context.TransportMessage.CorrelationId);

        var orderProcess = new OrderProcess(orderProcessId);

        var stream = _eventStore.Get(orderProcessId);

        if (stream.IsEmpty)
        {
            throw new ApplicationException(
                $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
        }

        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Order Created"));
        stream.AddEvent(orderProcess.AssignOrderId(context.Message.OrderId));

        _eventStore.Save(stream);

        context.Send(orderProcess.CreateInvoiceCommand());
    }
}