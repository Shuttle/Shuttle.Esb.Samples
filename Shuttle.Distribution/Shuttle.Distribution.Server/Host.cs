using Shuttle.Core.Container;
using Shuttle.Core.Unity;
using Shuttle.Core.WorkerService;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Unity;

namespace Shuttle.Distribution.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public void Start()
        {
            var container = new UnityComponentContainer(new UnityContainer());

            container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            container.RegisterServiceBus();

            _bus = container.Resolve<IServiceBus>().Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}