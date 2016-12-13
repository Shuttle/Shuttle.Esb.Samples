using System;
using log4net;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Log4Net;
using Shuttle.Esb;

namespace Shuttle.Deferred.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

            var container = new DefaultComponentContainer();

            DefaultConfigurator.Configure(container);

		    _bus = ServiceBus.Create(container).Start();
		}

        public void Dispose()
		{
			_bus.Dispose();
		}
	}
}