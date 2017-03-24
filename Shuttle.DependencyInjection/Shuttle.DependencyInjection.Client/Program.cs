using System;
using Ninject;
using Shuttle.Core.Ninject;
using Shuttle.DependencyInjection.Messages;
using Shuttle.Esb;

namespace Shuttle.DependencyInjection.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new NinjectComponentContainer(new StandardKernel());

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