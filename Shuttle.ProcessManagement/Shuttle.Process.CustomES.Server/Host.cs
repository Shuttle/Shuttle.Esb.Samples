using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Shuttle.Castle;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.EMailSender.Messages;
using Shuttle.ESB.Castle;
using Shuttle.ESB.Core;
using Shuttle.ESB.SqlServer;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Recall.Core;
using Shuttle.Recall.SqlServer;

namespace Shuttle.Process.CustomES.Server
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
            _container.Register(Component.For<Recall.Core.ISerializer>().ImplementedBy<Recall.Core.DefaultSerializer>());
            _container.Register(Component.For<IEventStoreQueryFactory>().ImplementedBy<EventStoreQueryFactory>());

            var subscriptionManager = SubscriptionManager.Default();

            subscriptionManager.Subscribe<OrderCreatedEvent>();
            subscriptionManager.Subscribe<InvoiceCreatedEvent>();
            subscriptionManager.Subscribe<EMailSentEvent>();

            _bus = ServiceBus.Create(
                c =>
                {
                    c
                        .MessageHandlerFactory(new CastleMessageHandlerFactory(_container).RegisterHandlers())
                        .SubscriptionManager(subscriptionManager);
                }).Start();
        }
    }
}