using System;
using Autofac;
using Shuttle.Core.Autofac;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Client
{
	class Program
	{
		static void Main(string[] args)
		{
            var containerBuilder = new ContainerBuilder();
            var registry = new AutofacComponentRegistry(containerBuilder);

            ServiceBusConfigurator.Configure(registry);

            using (var bus = ServiceBus.Create(new AutofacComponentResolver(containerBuilder.Build())).Start())
            {
                string userName;

				while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
				{
					bus.Send(new RegisterMemberCommand
					{
						UserName = userName
					}, c => c.Defer(DateTime.Now.AddSeconds(5)));
				}
			}
		}
	}
}
