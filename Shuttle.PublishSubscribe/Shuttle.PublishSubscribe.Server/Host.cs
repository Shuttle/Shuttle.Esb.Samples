using System;
using Shuttle.Core.Host;
using Shuttle.Core.StructureMap;
using Shuttle.Esb;
using StructureMap;

namespace Shuttle.PublishSubscribe.Server
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
			var smRegistry = new Registry();
			var registry = new StructureMapComponentRegistry(smRegistry);

			ServiceBus.Register(registry);

			_bus = ServiceBus.Create(new StructureMapComponentResolver(new Container(smRegistry))).Start();
		}
	}
}