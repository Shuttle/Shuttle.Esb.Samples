using System;
using Shuttle.Core.Contract;
using Shuttle.Esb;

namespace Shuttle.Process.QueryServer
{
    public static class TransportMessageExtensions
    {
        public static Guid OrderProcessId(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, nameof(transportMessage));

            return new Guid(transportMessage.CorrelationId);
        }
    }
}