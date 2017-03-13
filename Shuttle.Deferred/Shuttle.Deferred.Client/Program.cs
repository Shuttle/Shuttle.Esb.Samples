using System;
using Autofac;
using Shuttle.Core.Autofac;
using Shuttle.Deferred.Messages;
using Shuttle.Esb;

namespace Shuttle.Deferred.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var containerBuilder = new ContainerBuilder();
			var registry = new AutofacComponentRegistry(containerBuilder);

			ServiceBus.Register(registry);

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