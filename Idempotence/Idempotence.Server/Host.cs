using System;
using log4net;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;
using Shuttle.ESB.SqlServer;
using Shuttle.ESB.SqlServer.Idempotence;

namespace Idempotence.Server
{
	public class Host : IHost, IDisposable
	{
		private static IServiceBus _bus;

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (Host))));

			_bus = ServiceBus
				.Create(c => c.SubscriptionManager(SubscriptionManager.Default()).IdempotenceService(IdempotenceService.Default()))
				.Start();
		}

		public void Dispose()
		{
			_bus.Dispose();

			LogManager.Shutdown();
		}
	}
}