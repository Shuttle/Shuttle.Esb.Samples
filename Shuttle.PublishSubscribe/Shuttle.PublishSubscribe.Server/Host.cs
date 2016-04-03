using System;
using Shuttle.Core.Host;
using Shuttle.Esb;
using Shuttle.Esb.SqlServer;

namespace Shuttle.PublishSubscribe.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
			_bus = ServiceBus.Create(c => c.SubscriptionManager(SubscriptionManager.Default())).Start();
		}

		public void Dispose()
		{
			_bus.Dispose();
		}
	}
}