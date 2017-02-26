using System;
using Castle.Windsor;
using log4net;
using Shuttle.Castle;
using Shuttle.Core.Castle;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.Castle;
using Shuttle.Esb.Msmq;
using Shuttle.Esb.Process;
using Shuttle.Esb.Sql;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Recall;
using Shuttle.Recall.Sql;

namespace Shuttle.Process.ESModule.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;
		private IWindsorContainer _container;

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

			_container.RegisterDataAccessCore();
			_container.RegisterDataAccess("Shuttle.ProcessManagement");

			var container = new WindsorComponentContainer(_container);

			container.Register<Recall.Sql.IScriptProviderConfiguration, Recall.Sql.ScriptProviderConfiguration>();
			container.Register<Recall.Sql.IScriptProvider, Recall.Sql.ScriptProvider>();

			container.Register<IProjectionRepository, ProjectionRepository>();
			container.Register<IProjectionQueryFactory, ProjectionQueryFactory>();
			container.Register<IPrimitiveEventRepository, PrimitiveEventRepository>();
			container.Register<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();

			container.Register<IProjectionConfiguration>(ProjectionSection.Configuration());
			container.Register<EventProcessingModule, EventProcessingModule>();
			container.Register<ProcessModule, ProcessModule>();

			EventStoreConfigurator.Configure(container);

			var processConfiguration = ProcessSection.Configuration();

			((DefaultProcessActivator) processConfiguration.ProcessActivator).RegisterMappings();

			container.Register<IMsmqConfiguration, MsmqConfiguration>();

			var esbConfigurator = new ServiceBusConfigurator(container);

			esbConfigurator.DontRegister<ISubscriptionManager>();

			container.Register<Esb.Sql.IScriptProviderConfiguration, Esb.Sql.ScriptProviderConfiguration>();
			container.Register<Esb.Sql.IScriptProvider, Esb.Sql.ScriptProvider>();

			container.Register<ISqlConfiguration>(SqlSection.Configuration());
			container.Register<ISubscriptionManager, SubscriptionManager>();

			esbConfigurator.Configure();

			container.Resolve<EventProcessingModule>();
			container.Resolve<ProcessModule>();

			var subscriptionManager = container.Resolve<ISubscriptionManager>();

			subscriptionManager.Subscribe<OrderCreatedEvent>();
			subscriptionManager.Subscribe<InvoiceCreatedEvent>();
			subscriptionManager.Subscribe<EMailSentEvent>();

			_bus = ServiceBus.Create(container).Start();
		}
	}
}