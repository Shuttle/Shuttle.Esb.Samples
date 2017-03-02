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
			var smRegistry = new Registry();
			var registry = new StructureMapComponentRegistry(smRegistry);

			registry.Register<IMsmqConfiguration, MsmqConfiguration>();
			registry.Register<TransactionScopeObserver>();

			ServiceBusConfigurator.Configure(registry);

            using (var bus = ServiceBus.Create(new StructureMapComponentResolver(new Container(smRegistry))).Start())
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