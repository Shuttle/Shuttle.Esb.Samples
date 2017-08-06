using Shuttle.Core.ServiceHost;
using Shuttle.Core.SimpleInjector;
using Shuttle.Esb;
using SimpleInjector;

namespace Shuttle.Idempotence.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public void Start()
        {
            var container = new SimpleInjectorComponentContainer(new Container());

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}