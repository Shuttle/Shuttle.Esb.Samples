﻿using System;
using Shuttle.Core.Container;
using Shuttle.Core.SimpleInjector;
using Shuttle.Esb;
using Shuttle.Idempotence.Messages;
using SimpleInjector;

namespace Shuttle.Idempotence.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var container = new SimpleInjectorComponentContainer(new Container());

			ServiceBus.Register(container);

			var transportMessageFactory = container.Resolve<ITransportMessageFactory>();

			using (var bus = ServiceBus.Create(container).Start())
			{
				string userName;

				while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
				{
					var command = new RegisterMemberCommand
					{
						UserName = userName
					};

					var transportMessage = transportMessageFactory.Create(command, c => { });

					for (var i = 0; i < 5; i++)
					{
						bus.Dispatch(transportMessage); // will be processed once since message id is the same
					}

					bus.Send(command); // will be processed since it has a new message id
					bus.Send(command); // will also be processed since it too has a new message id
				}
			}
		}
	}
}