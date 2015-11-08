using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public class ProcessConfigurationObserver : IPipelineObserver<OnInitializing>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;
        private readonly IProcessConfiguration _configuration;

        public ProcessConfigurationObserver(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IProcessConfiguration configuration)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");
            Guard.AgainstNull(configuration, "configuration");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _configuration = configuration;
        }

        public void Execute(OnInitializing pipelineEvent)
        {
            pipelineEvent.Pipeline.State.GetServiceBus().Configuration.MessageHandlerInvoker =
                new ProcessMessageHandlerInvoker(_databaseContextFactory, _eventStore, _configuration);
        }
    }
}