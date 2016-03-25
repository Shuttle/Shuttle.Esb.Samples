using System;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;

namespace Shuttle.Process.CustomES.Server
{
    public static class TransportMessageExtensions
    {
        public static bool IsHandledHere(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, "transportMessage");

            return transportMessage.Headers.GetHeaderValue("TargetSystem")
                .Equals("custom / event-source", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}