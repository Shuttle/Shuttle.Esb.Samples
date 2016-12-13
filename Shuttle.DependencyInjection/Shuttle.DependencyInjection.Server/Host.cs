using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shuttle.Core.Castle;
using Shuttle.Core.Host;
using Shuttle.DependencyInjection.EMail;
using Shuttle.Esb;

namespace Shuttle.DependencyInjection.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;
		private WindsorContainer _container;

		public void Start()
		{
			_container = new WindsorContainer();

			_container.Register(Component.For<IEMailService>().ImplementedBy<EMailService>());

            var _componentContainer = new WindsorComponentContainer(_container);

            DefaultConfigurator.Configure(_componentContainer);

            _bus = ServiceBus.Create(_componentContainer).Start();
		}

		public void Dispose()
		{
			_bus.Dispose();
		}
	}
}