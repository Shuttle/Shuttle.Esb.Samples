using System;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.StructureMap;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;
using Shuttle.PublishSubscribe.Messages;
using StructureMap;

namespace Shuttle.PublishSubscribe.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			var registry = new Registry();
			var componentRegistry = new StructureMapComponentRegistry(registry);

			componentRegistry.Register<IMsmqConfiguration, MsmqConfiguration>();

			ServiceBusConfigurator.Configure(componentRegistry);

            using (var bus = ServiceBus.Create(new StructureMapComponentResolver(new Container(registry))).Start())
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