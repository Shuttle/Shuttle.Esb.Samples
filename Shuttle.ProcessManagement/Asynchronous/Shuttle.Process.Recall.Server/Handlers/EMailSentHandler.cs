using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class EMailSentHandler : IAsyncMessageHandler<EMailSent>
{
    private readonly IEventStore _eventStore;

    public EMailSentHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<EMailSent> context)
    {
        if (!context.TransportMessage.IsHandledHere())
        {
            return;
        }

        var orderProcessId = new Guid(context.TransportMessage.CorrelationId);

        var stream = await _eventStore.GetAsync(orderProcessId);

        if (stream.IsEmpty)
        {
            throw new ApplicationException(
                $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
        }

        var orderProcess = new OrderProcess(orderProcessId);
        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Dispatched-EMail Sent"));

        await _eventStore.SaveAsync(stream);

        await context.SendAsync(new CompleteOrderProcess
        {
            OrderProcessId = orderProcessId
        }, c => c.Local());
    }
}