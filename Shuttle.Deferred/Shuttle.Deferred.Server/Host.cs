using Autofac;
using log4net;
using Shuttle.Core.Autofac;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.ServiceHost;
using Shuttle.Esb;

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
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            var containerBuilder = new ContainerBuilder();
            var registry = new AutofacComponentRegistry(containerBuilder);

            ServiceBus.Register(registry);

            _bus = ServiceBus.Create(new AutofacComponentResolver(containerBuilder.Build())).Start();
        }
    }
}