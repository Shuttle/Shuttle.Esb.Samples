using Shuttle.Core.ServiceHost;
using Shuttle.Core.StructureMap;
using Shuttle.Esb;
using StructureMap;

namespace Shuttle.PublishSubscribe.Server
{
    public class Host : IServiceHostStart
    {
        private IServiceBus _bus;

        public void Start()
        {
            var smRegistry = new Registry();
            var registry = new StructureMapComponentRegistry(smRegistry);

            ServiceBus.Register(registry);

            _bus = ServiceBus.Create(new StructureMapComponentResolver(new Container(smRegistry))).Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}