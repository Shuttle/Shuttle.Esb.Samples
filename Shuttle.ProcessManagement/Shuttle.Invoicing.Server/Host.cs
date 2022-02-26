using Castle.Windsor;
using log4net;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.WorkerService;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.Invoicing.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;
        private WindsorContainer _container;

        public void Stop()
        {
            _bus?.Dispose();
        }

        public void Start()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            _container = new WindsorContainer();

            var container = new WindsorComponentContainer(_container);

            container.RegisterSuffixed("Shuttle.Invoicing");

            container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            container.RegisterServiceBus();

            _bus = container.Resolve<IServiceBus>().Start();
        }
    }
}