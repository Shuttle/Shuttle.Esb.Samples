using System;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Client
{
	class Program
	{
		static void Main(string[] args)
		{
            var container = new DefaultComponentContainer();

            DefaultConfigurator.Configure(container);

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

					var transportMessage = transportMessageFactory.Create(command, null);

					for (var i = 0; i < 5; i++)
					{
						bus.Dispatch(transportMessage); // will be processed once since message id is the same
					}

					bus.Send(command); // will be processed since it has a new message id
					bus.Send(command); // will also be processed since it also has a new message id
				}
			}
		}
	}
}