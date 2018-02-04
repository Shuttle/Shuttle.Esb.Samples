using System.Reflection;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Container;
using Shuttle.Core.Castle;
using Shuttle.Core.Data.Registration;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.ServiceHost;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;

namespace Shuttle.Process.Custom.Server
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

            container.RegisterDataAccess(Assembly.GetExecutingAssembly());

            ServiceBus.Register(container);

            var subscriptionManager = container.Resolve<ISubscriptionManager>();

            subscriptionManager.Subscribe<OrderCreatedEvent>();
            subscriptionManager.Subscribe<InvoiceCreatedEvent>();
            subscriptionManager.Subscribe<EMailSentEvent>();

            _bus = ServiceBus.Create(container).Start();
        }
    }
}