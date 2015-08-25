using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shuttle.Core.Host;
using Shuttle.DependencyInjection.EMail;
using Shuttle.ESB.Castle;
using Shuttle.ESB.Core;

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

			// register all the message handler in this assembly
			_container.Register(
				Classes.FromThisAssembly()
				.BasedOn(typeof(IMessageHandler<>))
				.WithServiceFromInterface(typeof(IMessageHandler<>))
				.LifestyleTransient()
				);

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