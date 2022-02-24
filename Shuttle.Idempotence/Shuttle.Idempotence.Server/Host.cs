using Shuttle.Core.Container;
using Shuttle.Core.Data;
using Shuttle.Core.SimpleInjector;
using Shuttle.Core.WorkerService;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.Esb.Sql.Idempotence;
using SimpleInjector;

namespace Shuttle.Idempotence.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public void Start()
        {
            var container = new SimpleInjectorComponentContainer(new Container());

            container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            container.RegisterDataAccess();
            container.RegisterIdempotence();
            container.RegisterServiceBus();

            _bus = container.Resolve<IServiceBus>().Start();
        }

        public void Stop()
        {
            _bus?.Dispose();
        }
    }
}