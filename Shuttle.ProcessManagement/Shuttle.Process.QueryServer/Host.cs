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
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;
        private WindsorContainer _container;

        public void Stop()
        {
            _bus?.Dispose();
            _container?.Dispose();
        }

        public void Start()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            _container = new WindsorContainer();

            var container = new WindsorComponentContainer(_container);

            container.RegisterDataAccess("Shuttle.ProcessManagement");

            ServiceBus.Register(container);

            var subscriptionManager = container.Resolve<ISubscriptionManager>();

            subscriptionManager.Subscribe<OrderProcessRegisteredEvent>();
            subscriptionManager.Subscribe<OrderProcessCancelledEvent>();
            subscriptionManager.Subscribe<OrderProcessAcceptedEvent>();
            subscriptionManager.Subscribe<OrderProcessCompletedEvent>();
            subscriptionManager.Subscribe<OrderProcessArchivedEvent>();
            subscriptionManager.Subscribe<OrderCreatedEvent>();
            subscriptionManager.Subscribe<CancelOrderProcessRejectedEvent>();
            subscriptionManager.Subscribe<ArchiveOrderProcessRejectedEvent>();
            subscriptionManager.Subscribe<InvoiceCreatedEvent>();
            subscriptionManager.Subscribe<EMailSentEvent>();

            _bus = ServiceBus.Create(container).Start();
        }
    }
}