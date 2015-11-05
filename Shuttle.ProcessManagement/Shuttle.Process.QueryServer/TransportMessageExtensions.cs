using System;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Process.QueryServer
{
    public static class TransportMessageExtensions
    {
        public static Guid OrderProcessId(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, "transportMessage");

            return new Guid(transportMessage.CorrelationId);
        }
    }
}