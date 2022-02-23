using System;
using Shuttle.Core.Container;
using Shuttle.Core.StructureMap;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.PublishSubscribe.Messages;
using StructureMap;

namespace Shuttle.PublishSubscribe.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var registry = new Registry();
			var componentRegistry = new StructureMapComponentRegistry(registry);

			componentRegistry.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
			componentRegistry.RegisterServiceBus();

			using (var bus = new StructureMapComponentResolver(new Container(registry)).Resolve<IServiceBus>().Start())
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