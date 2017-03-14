using System;
using Castle.Windsor;
using log4net;
using Shuttle.Castle;
using Shuttle.Core.Castle;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;
using Shuttle.Esb.Sql;
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
			if (_bus != null)
			{
				_bus.Dispose();
			}

			if (_container != null)
			{
				_container.Dispose();
			}
		}

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

			_container = new WindsorContainer();

			_container.RegisterDataAccess("Shuttle.ProcessManagement");

			var container = new WindsorComponentContainer(_container);

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