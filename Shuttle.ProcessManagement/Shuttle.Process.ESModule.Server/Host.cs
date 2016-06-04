using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Shuttle.Castle;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb.Castle;
using Shuttle.Esb;
using Shuttle.Esb.Process;
using Shuttle.Esb.SqlServer;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Recall;
using Shuttle.Recall.SqlServer;

namespace Shuttle.Process.ESModule.Server
{
    public class Host : IHost, IDisposable
    {
        private IServiceBus _bus;
        private WindsorContainer _container;

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void Start()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (Host))));

            _container = new WindsorContainer();

            _container.RegisterDataAccessCore();
            _container.RegisterDataAccess("Shuttle.ProcessManagement");

            _container.Register(Component.For<IEventStore>().ImplementedBy<EventStore>());
            _container.Register(Component.For<IKeyStore>().ImplementedBy<KeyStore>());
            _container.Register(Component.For<ISerializer>().ImplementedBy<DefaultSerializer>());
            _container.Register(Component.For<IEventStoreQueryFactory>().ImplementedBy<EventStoreQueryFactory>());
            _container.Register(Component.For<IKeyStoreQueryFactory>().ImplementedBy<KeyStoreQueryFactory>());

            var subscriptionManager = SubscriptionManager.Default();

            subscriptionManager.Subscribe<OrderCreatedEvent>();
            subscriptionManager.Subscribe<InvoiceCreatedEvent>();
            subscriptionManager.Subscribe<EMailSentEvent>();

            var processConfiguration = ProcessSection.Configuration();

	        ((DefaultProcessActivator)processConfiguration.ProcessActivator).RegisterMappings();

            _bus = ServiceBus.Create(
                c =>
                {
                    c.MessageHandlerFactory(new CastleMessageHandlerFactory(_container));
                    c.SubscriptionManager(subscriptionManager);
                    c.AddModule(new ProcessModule(_container.Resolve<IDatabaseContextFactory>(), _container.Resolve<IEventStore>(), _container.Resolve<IKeyStore>(), processConfiguration));
                }).Start();
        }
    }
}