using System;
using Autofac;
using Shuttle.Core.Autofac;
using Shuttle.Core.Container;
using Shuttle.Deferred.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.Deferred.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var containerBuilder = new ContainerBuilder();
			var registry = new AutofacComponentRegistry(containerBuilder);

			registry.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
			registry.RegisterServiceBus();

			using (var bus = new AutofacComponentResolver(containerBuilder.Build()).Resolve<IServiceBus>().Start())
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