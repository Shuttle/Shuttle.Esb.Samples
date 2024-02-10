using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Esb;
using System;

namespace Shuttle.Deferred.Server
{
    public class DeferredModule
    {
        private readonly IServiceBusConfiguration _serviceBusConfiguration;
        private readonly Type _startupPipelineType = typeof(StartupPipeline);

        public DeferredModule(IPipelineFactory pipelineFactory, IServiceBusConfiguration serviceBusConfiguration )
        {
            _serviceBusConfiguration = serviceBusConfiguration;
            Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));

            pipelineFactory.PipelineCreated += PipelineCreated;
        }

        private void PipelineCreated(object sender, PipelineEventArgs e)
        {
            if (e.Pipeline.GetType() != _startupPipelineType)
            {
                return;
            }

            e.Pipeline.RegisterObserver(new DeferredObserver(_serviceBusConfiguration));
        }

    }
}