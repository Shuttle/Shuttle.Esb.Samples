using System;
using Autofac;
using log4net;
using Shuttle.Core.Autofac;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;

namespace Shuttle.Deferred.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

		    var containerBuilder = new ContainerBuilder();
		    var registry = new AutofacComponentRegistry(containerBuilder);

			registry.Register<IMsmqConfiguration, MsmqConfiguration>();

            ServiceBusConfigurator.Configure(registry);

		    _bus = ServiceBus.Create(new AutofacComponentResolver(containerBuilder.Build())).Start();
		}

        public void Dispose()
		{
			_bus.Dispose();
		}
	}
}