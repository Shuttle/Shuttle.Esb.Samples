using System;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;

namespace Shuttle.Process.ESModule.Server
{
    public static class TransportMessageExtensions
    {
        public static bool IsHandledHere(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, "transportMessage");

            return transportMessage.Headers.GetHeaderValue("TargetSystem")
                .Equals("event-source / module", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}