using System;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.ESB.Process
{
    public static class TransportMessageExtensions
    {
        public static string GetProcessAssemblyQualifiedName(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, "transportMessage");

            return transportMessage.Headers.GetHeaderValue("ProcessAssemblyQualifiedName");
        }      
        
        public static string GetProcessTypeName(this TransportMessage transportMessage)
        {
            Guard.AgainstNull(transportMessage, "transportMessage");

            return transportMessage.Headers.GetHeaderValue("ProcessTypeName");
        }        
        
        public static void SetProcessType(this TransportMessage transportMessage, object processInstance)
        {
            Guard.AgainstNull(transportMessage, "transportMessage");
            Guard.AgainstNull(processInstance, "processInstance");

            var type = processInstance.GetType();

            transportMessage.Headers.SetHeaderValue("ProcessAssemblyQualifiedName", type.AssemblyQualifiedName);
            transportMessage.Headers.SetHeaderValue("ProcessTypeName", type.FullName);
        }
    }
}