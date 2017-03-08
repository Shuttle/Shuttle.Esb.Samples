using System;
using Microsoft.Practices.Unity;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Unity;
using Shuttle.Distribution.Messages;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;

namespace Shuttle.Distribution.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new UnityComponentContainer(new UnityContainer());

            container.Register<IMsmqConfiguration, MsmqConfiguration>();

			ServiceBus.Register(container);

            using (var bus = ServiceBus.Create(container).Start())
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