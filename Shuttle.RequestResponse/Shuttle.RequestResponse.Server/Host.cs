using Castle.Windsor;
using log4net;
using Shuttle.Core.Castle;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.Core.ServiceHost;
using Shuttle.Esb;

namespace Shuttle.RequestResponse.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public Host()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));
        }

        public void Start()
        {
            var container = new WindsorComponentContainer(new WindsorContainer());

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}