﻿using Ninject;
using Shuttle.Core.Ninject;
using Shuttle.Core.ServiceHost;
using Shuttle.DependencyInjection.EMail;
using Shuttle.Esb;

namespace Shuttle.DependencyInjection.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;
        private StandardKernel _kernel;

        public void Stop()
        {
            _kernel.Dispose();
            _bus.Dispose();
        }

        public void Start()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<IEMailService>().To<EMailService>();

            var container = new NinjectComponentContainer(_kernel);

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();
        }
    }
}