using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class ArchiveOrderProcessHandler : IAsyncMessageHandler<ArchiveOrderProcess>
{
    private readonly IEventStore _eventStore;

    public ArchiveOrderProcessHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<ArchiveOrderProcess> context)
    {
        var stream = await _eventStore.GetAsync(context.Message.OrderProcessId);
        var orderProcess = new OrderProcess(context.Message.OrderProcessId);

        stream.Apply(orderProcess);

        if (!orderProcess.CanArchive)
        {
            await context.PublishAsync(new ArchiveOrderProcessRejected
            {
                OrderProcessId = context.Message.OrderProcessId,
                Status = orderProcess.Status
            });

            return;
        }

        stream.AddEvent(orderProcess.ChangeStatus("Order Archived"));

        await _eventStore.SaveAsync(stream);

        await context.PublishAsync(new OrderProcessArchived
        {
            OrderProcessId = context.Message.OrderProcessId
        });
    }
}