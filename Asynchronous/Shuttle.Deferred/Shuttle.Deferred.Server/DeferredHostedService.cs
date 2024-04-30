﻿using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Esb;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Shuttle.Deferred.Server
{
    public class DeferredHostedService : IHostedService
    {
        private readonly IDeferredMessageProcessor _deferredMessageProcessor;

        public DeferredHostedService(IDeferredMessageProcessor deferredMessageProcessor)
        {
            _deferredMessageProcessor = Guard.AgainstNull(deferredMessageProcessor, nameof(deferredMessageProcessor));
        }

        private void OnDeferredMessageProcessingAdjusted(object sender, DeferredMessageProcessingAdjustedEventArgs args)
        {
            Console.WriteLine($"[deferred processing adjusted] : next = {args.NextProcessingDateTime}");
        }

        private void OnDeferredMessageProcessingHalted(object sender, DeferredMessageProcessingHaltedEventArgs args)
        {
            Console.WriteLine($"[deferred processing halted] : until = {args.RestartDateTime}");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _deferredMessageProcessor.DeferredMessageProcessingHalted += OnDeferredMessageProcessingHalted;
            _deferredMessageProcessor.DeferredMessageProcessingAdjusted += OnDeferredMessageProcessingAdjusted;

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _deferredMessageProcessor.DeferredMessageProcessingHalted -= OnDeferredMessageProcessingHalted;
            _deferredMessageProcessor.DeferredMessageProcessingAdjusted -= OnDeferredMessageProcessingAdjusted;

            await Task.CompletedTask;
        }
    }
}