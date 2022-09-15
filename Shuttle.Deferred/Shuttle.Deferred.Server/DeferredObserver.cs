using System;
using Shuttle.Core.Pipelines;
using Shuttle.Esb;

namespace Shuttle.Deferred.Server
{
    public class DeferredObserver : IPipelineObserver<OnStartDeferredMessageProcessing>
    {
        private readonly IServiceBusConfiguration _serviceBusConfiguration;

        public DeferredObserver(IServiceBusConfiguration serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
        }

        public void Execute(OnStartDeferredMessageProcessing pipelineEvent)
        {
            _serviceBusConfiguration.Inbox.DeferredMessageProcessor
                    .DeferredMessageProcessingHalted +=
                (sender, args) =>
                {
                    Console.WriteLine($"[deferred processing halted] : until = {args.RestartDateTime}");
                };
            _serviceBusConfiguration.Inbox.DeferredMessageProcessor
                    .DeferredMessageProcessingAdjusted +=
                (sender, args) =>
                {
                    Console.WriteLine($"[deferred processing adjusted] : next = {args.NextProcessingDateTime}");
                };
        }
    }
}