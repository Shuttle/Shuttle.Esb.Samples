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

namespace Shuttle.EMailSender.Server
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

			var container = new WindsorComponentContainer(_container);

			_container.RegisterDataAccessCore();

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