using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class CompleteOrderProcessHandler : IAsyncMessageHandler<CompleteOrderProcess>
{
    private readonly IEventStore _eventStore;

    public CompleteOrderProcessHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<CompleteOrderProcess> context)
    {
        var stream = await _eventStore.GetAsync(context.Message.OrderProcessId);
        var orderProcess = new OrderProcess(context.Message.OrderProcessId);
        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Completed"));

        await _eventStore.SaveAsync(stream);

        await context.PublishAsync(new OrderProcessCompleted
        {
            OrderProcessId = orderProcess.Id
        });
    }
}