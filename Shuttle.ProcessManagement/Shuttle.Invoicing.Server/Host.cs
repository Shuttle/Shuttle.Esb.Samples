using System;
using Castle.Windsor;
using log4net;
using Shuttle.Castle;
using Shuttle.Core.Castle;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;
using Shuttle.Esb.Sql;

namespace Shuttle.Invoicing.Server
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
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

			_container = new WindsorContainer();

			_container.RegisterDataAccessCore();
			_container.RegisterDataAccess("Shuttle.Invoicing");

			var container = new WindsorComponentContainer(_container);

			var configurator = new ServiceBusConfigurator(container);

			configurator.DontRegister<ISubscriptionManager>();

			container.Register<IMsmqConfiguration, MsmqConfiguration>();

			container.Register<IScriptProvider, ScriptProvider>();
			container.Register<IScriptProviderConfiguration, ScriptProviderConfiguration>();

			container.Register<ISqlConfiguration>(SqlSection.Configuration());
			container.Register<ISubscriptionManager, SubscriptionManager>();

			configurator.Configure();

			_bus = ServiceBus.Create(container).Start();
		}
	}
}