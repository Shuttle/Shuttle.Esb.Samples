using System;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;

namespace Shuttle.Distribution.Worker
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
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