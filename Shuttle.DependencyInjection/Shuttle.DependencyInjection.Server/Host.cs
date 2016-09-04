using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shuttle.Core.Host;
using Shuttle.DependencyInjection.EMail;
using Shuttle.Esb.Castle;
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

            // also calls RegisterHandlers to register handlers in this assembly
			_bus = ServiceBus.Create(
				c => c.MessageHandlerFactory(new CastleMessageHandlerFactory(_container))
				).Start();
		}

		public void Dispose()
		{
			_bus.Dispose();
		}
	}
}