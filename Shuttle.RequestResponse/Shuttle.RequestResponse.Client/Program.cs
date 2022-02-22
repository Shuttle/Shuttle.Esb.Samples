using System;
using Castle.Windsor;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var container = new WindsorComponentContainer(new WindsorContainer());

			container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
			container.RegisterServiceBus();

			using (var bus = container.Resolve<IServiceBus>().Start())
			{
				string userName;

				while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
				{
					bus.Send(new RegisterMemberCommand
					{
						UserName = userName
					}, c => c.WillExpire(DateTime.Now.AddSeconds(5)));
				}
			}
		}
	}
}