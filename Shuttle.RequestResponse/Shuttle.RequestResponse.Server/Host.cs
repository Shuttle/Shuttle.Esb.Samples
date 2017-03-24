using System;
using Castle.Windsor;
using Shuttle.Core.Castle;
using Shuttle.Core.Host;
using Shuttle.Esb;

namespace Shuttle.RequestResponse.Server
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
            var container = new WindsorComponentContainer(new WindsorContainer());

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();
        }
    }
}