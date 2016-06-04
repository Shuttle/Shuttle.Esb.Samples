using System;
using System.Reflection;
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

namespace Shuttle.Process.Custom.Server
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
            _container.RegisterDataAccess(Assembly.GetExecutingAssembly());

            var subscriptionManager = SubscriptionManager.Default();

            subscriptionManager.Subscribe<OrderCreatedEvent>();
            subscriptionManager.Subscribe<InvoiceCreatedEvent>();
            subscriptionManager.Subscribe<EMailSentEvent>();

            _bus = ServiceBus.Create(
                c =>
                {
                    c
                        .MessageHandlerFactory(new CastleMessageHandlerFactory(_container))
                        .SubscriptionManager(subscriptionManager);
                }).Start();
        }
    }
}