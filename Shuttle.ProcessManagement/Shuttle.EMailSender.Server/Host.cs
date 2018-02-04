using Castle.Windsor;
using log4net;
using Shuttle.Core.Castle;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.ServiceHost;
using Shuttle.Esb;

namespace Shuttle.EMailSender.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;
        private WindsorContainer _container;

        public void Start()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            _container = new WindsorContainer();

            var container = new WindsorComponentContainer(_container);

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();
        }

        public void Stop()
        {
            _bus?.Dispose();
            _container?.Dispose();
        }
    }
}