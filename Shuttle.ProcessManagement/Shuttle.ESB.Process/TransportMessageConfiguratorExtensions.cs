using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.ESB.Process
{
    public static class TransportMessageConfiguratorExtensions
    {
        public static TransportMessageConfigurator WithProcess(this TransportMessageConfigurator configurator, IProcessManager process)
        {
            Guard.AgainstNull(configurator, "configurator");
            Guard.AgainstNull(process, "process");

            var type = process.GetType();

            configurator.Headers.SetHeaderValue("ProcessAssemblyQualifiedName", type.AssemblyQualifiedName);
            configurator.Headers.SetHeaderValue("ProcessTypeName", type.FullName);

            return configurator.WithCorrelationId(process.CorrelationId.ToString("n"));
        }
    }
}