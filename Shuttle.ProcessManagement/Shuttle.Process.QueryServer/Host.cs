using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Shuttle.Castle;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb.Castle;
using Shuttle.Esb;
using Shuttle.Esb.SqlServer;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer
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

            var subscriptionManager = SubscriptionManager.Default();

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