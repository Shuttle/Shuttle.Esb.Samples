using System;
using Shuttle.Core.Contract;
using Shuttle.Esb;

namespace Shuttle.Process.Recall.Server
{
    public static class TransportMessageExtensions
    {
        public static bool IsHandledHere(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, nameof(transportMessage));

            return transportMessage.Headers.GetHeaderValue("TargetSystem")
                .Equals("event-source", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}