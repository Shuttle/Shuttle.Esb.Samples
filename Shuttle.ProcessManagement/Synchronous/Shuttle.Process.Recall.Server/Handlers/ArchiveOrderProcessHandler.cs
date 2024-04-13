using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class ArchiveOrderProcessHandler : IMessageHandler<ArchiveOrderProcess>
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IEventStore _eventStore;

    public ArchiveOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
    {
        Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        Guard.AgainstNull(eventStore, nameof(eventStore));

        _databaseContextFactory = databaseContextFactory;
        _eventStore = eventStore;
    }

    public void ProcessMessage(IHandlerContext<ArchiveOrderProcess> context)
    {
        var stream = _eventStore.Get(context.Message.OrderProcessId);
        var orderProcess = new OrderProcess(context.Message.OrderProcessId);

        stream.Apply(orderProcess);

        if (!orderProcess.CanArchive)
        {
            context.Publish(new ArchiveOrderProcessRejected
            {
                OrderProcessId = context.Message.OrderProcessId,
                Status = orderProcess.Status
            });

            return;
        }

        stream.AddEvent(orderProcess.ChangeStatus("Order Archived"));

        _eventStore.Save(stream);

        context.Publish(new OrderProcessArchived
        {
            OrderProcessId = context.Message.OrderProcessId
        });
    }
}