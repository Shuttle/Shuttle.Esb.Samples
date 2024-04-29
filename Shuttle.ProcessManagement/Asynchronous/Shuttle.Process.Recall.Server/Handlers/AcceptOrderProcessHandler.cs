using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class AcceptOrderProcessHandler : IAsyncMessageHandler<AcceptOrderProcess>
{
    private readonly IEventStore _eventStore;

    public AcceptOrderProcessHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<AcceptOrderProcess> context)
    {
        var stream = await _eventStore.GetAsync(context.Message.OrderProcessId);

        if (stream.IsEmpty)
        {
            return;
        }

        var orderProcess = new OrderProcess(context.Message.OrderProcessId);

        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Order Accepted"));

        await _eventStore.SaveAsync(stream);

        await context.SendAsync(orderProcess.CreateOrderCommand(), c => c.WithCorrelationId(orderProcess.Id.ToString()));

        await context.PublishAsync(new OrderProcessAccepted
        {
            OrderProcessId = orderProcess.Id
        });
    }
}