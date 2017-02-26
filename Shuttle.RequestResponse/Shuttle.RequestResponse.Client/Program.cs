using System;
using Castle.Windsor;
using Shuttle.Core.Castle;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorComponentContainer(new WindsorContainer());

	        container.Register<IMsmqConfiguration, MsmqConfiguration>();

            ServiceBusConfigurator.Configure(container);

            using (var bus = ServiceBus.Create(container).Start())
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
