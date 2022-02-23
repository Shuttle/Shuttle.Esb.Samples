using Shuttle.Core.Container;
using Shuttle.Core.Data;
using Shuttle.Core.StructureMap;
using Shuttle.Core.WorkerService;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.PublishSubscribe.Messages;
using StructureMap;

namespace Shuttle.PublishSubscribe.Subscriber
{
    public class Host : IServiceHostStart
    {
        private IServiceBus _bus;

        public void Start()
        {
            var registry = new Registry();
            var componentRegistry = new StructureMapComponentRegistry(registry);

            componentRegistry.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            componentRegistry.RegisterDataAccess();
            componentRegistry.RegisterSubscription();
            componentRegistry.RegisterServiceBus();

            var resolver = new StructureMapComponentResolver(new Container(registry));

            resolver.Resolve<ISubscriptionManager>().Subscribe<MemberRegisteredEvent>();

            _bus = resolver.Resolve<IServiceBus>().Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}