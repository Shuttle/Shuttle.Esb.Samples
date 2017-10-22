using Shuttle.Core.ServiceHost;
using Shuttle.Core.Unity;
using Shuttle.Esb;
using Unity;

namespace Shuttle.Distribution.Worker
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public void Start()
        {
            var container = new UnityComponentContainer(new UnityContainer());

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}