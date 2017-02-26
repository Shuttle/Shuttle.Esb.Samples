using System;
using Ninject;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Ninject;
using Shuttle.Esb;
using Shuttle.DependencyInjection.Messages;
using Shuttle.Esb.Msmq;

namespace Shuttle.DependencyInjection.Client
{
	class Program
	{
		static void Main(string[] args)
		{
            var container = new NinjectComponentContainer(new StandardKernel());

			container.Register<IMsmqConfiguration, MsmqConfiguration>();

            ServiceBusConfigurator.Configure(container);

            using (var bus = ServiceBus.Create(container).Start())
            {
                string userName;

				while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
				{
					bus.Send(new RegisterMemberCommand
					{
						UserName = userName
					});
				}
			}
		}
	}
}