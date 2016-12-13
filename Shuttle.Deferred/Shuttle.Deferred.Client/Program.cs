using System;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Client
{
	class Program
	{
		static void Main(string[] args)
		{
            var container = new DefaultComponentContainer();

            DefaultConfigurator.Configure(container);

            using (var bus = ServiceBus.Create(container).Start())
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
