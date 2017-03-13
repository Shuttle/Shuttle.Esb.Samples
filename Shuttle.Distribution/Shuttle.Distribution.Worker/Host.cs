using System;
using Microsoft.Practices.Unity;
using Shuttle.Core.Host;
using Shuttle.Core.Unity;
using Shuttle.Esb;

namespace Shuttle.Distribution.Worker
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
			var container = new UnityComponentContainer(new UnityContainer());

			ServiceBus.Register(container);

			_bus = ServiceBus.Create(container).Start();
		}
	}
}