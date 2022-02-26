using Castle.Windsor;
using log4net;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
using Shuttle.Core.Data;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.WorkerService;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.ESModule.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;
        private IWindsorContainer _container;

        public void Start()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            _container = new WindsorContainer();

            var container = new WindsorComponentContainer(_container);

            container.RegisterSuffixed("Shuttle.ProcessManagement");

            container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            container.RegisterDataAccess();
            container.RegisterSubscription();
            container.RegisterEventStore();
            container.RegisterServiceBus();

            var subscriptionManager = container.Resolve<ISubscriptionManager>();

            subscriptionManager.Subscribe<OrderCreatedEvent>();
            subscriptionManager.Subscribe<InvoiceCreatedEvent>();
            subscriptionManager.Subscribe<EMailSentEvent>();

            _bus = container.Resolve<IServiceBus>().Start();
        }

        public void Stop()
        {
            _bus?.Dispose();
            _container?.Dispose();
        }
    }
}