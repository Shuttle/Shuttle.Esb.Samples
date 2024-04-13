using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class CompleteOrderProcessHandler : IMessageHandler<CompleteOrderProcess>
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IEventStore _eventStore;

    public CompleteOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
    {
        Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        Guard.AgainstNull(eventStore, nameof(eventStore));

        _databaseContextFactory = databaseContextFactory;
        _eventStore = eventStore;
    }

    public void ProcessMessage(IHandlerContext<CompleteOrderProcess> context)
    {
        OrderProcess orderProcess;

        var stream = _eventStore.Get(context.Message.OrderProcessId);
        orderProcess = new OrderProcess(context.Message.OrderProcessId);
        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Completed"));

        _eventStore.Save(stream);

        context.Publish(new OrderProcessCompleted
        {
            OrderProcessId = orderProcess.Id
        });
    }
}