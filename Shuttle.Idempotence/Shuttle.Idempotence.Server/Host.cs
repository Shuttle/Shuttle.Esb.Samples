using System;
using Shuttle.Core.Host;
using Shuttle.Core.SimpleInjector;
using Shuttle.Esb;
using SimpleInjector;

namespace Shuttle.Idempotence.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Dispose()
		{
			_bus.Dispose();
		}

		public void Start()
		{
			var container = new SimpleInjectorComponentContainer(new Container());

			ServiceBus.Register(container);

			_bus = ServiceBus.Create(container).Start();
		}
	}
}