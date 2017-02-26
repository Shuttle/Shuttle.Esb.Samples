using System;
using Ninject;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Ninject;
using Shuttle.DependencyInjection.EMail;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;

namespace Shuttle.DependencyInjection.Server
{
    public class Host : IHost, IDisposable
    {
        private IServiceBus _bus;
        private StandardKernel _kernel;

        public void Dispose()
        {
            _kernel.Dispose();
            _bus.Dispose();
        }

        public void Start()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<IEMailService>().To<EMailService>();

            var container = new NinjectComponentContainer(_kernel);

			container.Register<IMsmqConfiguration, MsmqConfiguration>();

			ServiceBusConfigurator.Configure(container);

            _bus = ServiceBus.Create(container).Start();
        }
    }
}