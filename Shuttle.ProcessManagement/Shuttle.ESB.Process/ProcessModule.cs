using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public class ProcessModule : IModule
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;
        private readonly IProcessConfiguration _configuration;

        private readonly string _startupPipelineName = typeof (StartupPipeline).FullName;
        private IServiceBus _bus;

        public ProcessModule(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IProcessConfiguration configuration)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");
            Guard.AgainstNull(configuration, "configuration");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _configuration = configuration;
        }

        public void Initialize(IServiceBus bus)
        {
            _bus = bus;

            _bus.Events.PipelineCreated += PipelineCreated;
        }

        private void PipelineCreated(object sender, PipelineEventArgs e)
        {
            if (!e.Pipeline.GetType().FullName.Equals(_startupPipelineName, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            e.Pipeline.RegisterObserver(new ProcessConfigurationObserver(_databaseContextFactory, _eventStore, _configuration));
        }
    }
}