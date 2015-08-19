using System;
using Shuttle.Core.Host;
using Shuttle.ESB.Core;
using Shuttle.ESB.SqlServer;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Subscriber
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
			var subscriptionManager = SubscriptionManager.Default();

			subscriptionManager.Subscribe<MemberRegisteredEvent>();

			_bus = ServiceBus.Create(c => c.SubscriptionManager(subscriptionManager)).Start();
		}

		public void Dispose()
		{
			_bus.Dispose();
		}
	}
}
