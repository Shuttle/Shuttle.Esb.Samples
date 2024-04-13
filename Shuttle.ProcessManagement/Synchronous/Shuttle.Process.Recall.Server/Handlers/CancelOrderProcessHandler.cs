using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class CancelOrderProcessHandler : IMessageHandler<CancelOrderProcess>
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IEventStore _eventStore;

    public CancelOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
    {
        Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        Guard.AgainstNull(eventStore, nameof(eventStore));

        _databaseContextFactory = databaseContextFactory;
        _eventStore = eventStore;
    }

    public void ProcessMessage(IHandlerContext<CancelOrderProcess> context)
    {
        var stream = _eventStore.Get(context.Message.OrderProcessId);
        var orderProcess = new OrderProcess(context.Message.OrderProcessId);

        stream.Apply(orderProcess);

        if (!orderProcess.CanCancel)
        {
            context.Publish(new CancelOrderProcessRejected
            {
                OrderProcessId = context.Message.OrderProcessId,
                Status = orderProcess.Status
            });

            return;
        }

        _eventStore.Remove(context.Message.OrderProcessId);

        context.Publish(new OrderProcessCancelled
        {
            OrderProcessId = context.Message.OrderProcessId
        });
    }
}