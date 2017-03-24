using System;
using Microsoft.Practices.Unity;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Unity;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;

namespace Shuttle.Distribution.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
            var container = new UnityComponentContainer(new UnityContainer());

			ServiceBus.Register(container);

			_bus = ServiceBus.Create(container).Start();
        }

        public void Dispose()
		{
			_bus.Dispose();
		}
	}
}