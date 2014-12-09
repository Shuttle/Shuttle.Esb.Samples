using System;
using Idempotence.Messages;
using log4net;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;
using Shuttle.ESB.SqlServer;
using Shuttle.ESB.SqlServer.Idempotence;

namespace Idempotence.Subscriber
{
	public class Host : IHost, IDisposable
	{
		private static IServiceBus _bus;

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));

			var subscriptionManager = SubscriptionManager.Default();

			subscriptionManager.Subscribe<MessageShownEvent>();

			_bus = ServiceBus
				.Create(c => c.SubscriptionManager(subscriptionManager).IdempotenceService(IdempotenceService.Default()))
				.Start();
		}

		public void Dispose()
		{
			_bus.Dispose();

			LogManager.Shutdown();
		}
	}
}