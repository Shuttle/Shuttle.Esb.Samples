using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class CancelOrderProcessHandler : IMessageHandler<CancelOrderProcessCommand>
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

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<CancelOrderProcessCommand> context)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var stream = _eventStore.Get(context.Message.OrderProcessId);
                var orderProcess = new OrderProcess(context.Message.OrderProcessId);
                stream.Apply(orderProcess);

                if (!orderProcess.CanCancel)
                {
                    context.Publish(new CancelOrderProcessRejectedEvent
                    {
                        OrderProcessId = context.Message.OrderProcessId,
                        Status = orderProcess.Status
                    });

                    return;
                }

                _eventStore.Remove(context.Message.OrderProcessId);
            }

            context.Publish(new OrderProcessCancelledEvent
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}