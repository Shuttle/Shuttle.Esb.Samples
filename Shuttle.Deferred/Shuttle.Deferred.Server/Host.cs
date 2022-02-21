using System.Text;
using Autofac;
using log4net;
using Shuttle.Core.Autofac;
using Shuttle.Core.Container;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.WorkerService;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.Deferred.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public void Stop()
        {
            _bus.Dispose();
        }

        public void Start()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            var containerBuilder = new ContainerBuilder();
            var registry = new AutofacComponentRegistry(containerBuilder);

            registry.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            registry.RegisterServiceBus();

            _bus = new AutofacComponentResolver(containerBuilder.Build()).Resolve<IServiceBus>().Start();
        }
    }
}