using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class CancelOrderProcessHandler : IAsyncMessageHandler<CancelOrderProcess>
{
    private readonly IEventStore _eventStore;

    public CancelOrderProcessHandler(IEventStore eventStore)
    {
        _eventStore = Guard.AgainstNull(eventStore, nameof(eventStore));
    }

    public async Task ProcessMessageAsync(IHandlerContext<CancelOrderProcess> context)
    {
        var stream = await _eventStore.GetAsync(context.Message.OrderProcessId);
        var orderProcess = new OrderProcess(context.Message.OrderProcessId);

        stream.Apply(orderProcess);

        if (!orderProcess.CanCancel)
        {
            await context.PublishAsync(new CancelOrderProcessRejected
            {
                OrderProcessId = context.Message.OrderProcessId,
                Status = orderProcess.Status
            });

            return;
        }

        await _eventStore.RemoveAsync(context.Message.OrderProcessId);

        await context.PublishAsync(new OrderProcessCancelled
        {
            OrderProcessId = context.Message.OrderProcessId
        });
    }
}