using Shuttle.Core.Infrastructure;
using Shuttle.Core.ServiceHost;
using Shuttle.Core.StructureMap;
using Shuttle.Esb;
using Shuttle.PublishSubscribe.Messages;
using StructureMap;

namespace Shuttle.PublishSubscribe.Subscriber
{
    public class Host : IServiceHostStart
    {
        private IServiceBus _bus;

        public void Start()
        {
            var structureMapRegistry = new Registry();
            var registry = new StructureMapComponentRegistry(structureMapRegistry);

            ServiceBus.Register(registry);

            var resolver = new StructureMapComponentResolver(new Container(structureMapRegistry));

            resolver.Resolve<ISubscriptionManager>().Subscribe<MemberRegisteredEvent>();

            _bus = ServiceBus.Create(resolver).Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}