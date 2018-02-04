using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.CustomES.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.CustomES.Server
{
    public class CompleteOrderProcessHandler : IMessageHandler<CompleteOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;

        public CompleteOrderProcessHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<CompleteOrderProcessCommand> context)
        {
            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var stream = _eventStore.Get(context.Message.OrderProcessId);
                orderProcess = new OrderProcess(context.Message.OrderProcessId);
                stream.Apply(orderProcess);

                stream.AddEvent(orderProcess.ChangeStatus("Completed"));

                _eventStore.Save(stream);
            }

            context.Publish(new OrderProcessCompletedEvent
            {
                OrderProcessId = orderProcess.Id
            });
        }
    }
}