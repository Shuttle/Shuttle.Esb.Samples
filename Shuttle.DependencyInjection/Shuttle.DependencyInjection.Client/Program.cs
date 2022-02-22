using System;
using Ninject;
using Shuttle.Core.Container;
using Shuttle.Core.Ninject;
using Shuttle.DependencyInjection.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.DependencyInjection.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new NinjectComponentContainer(new StandardKernel());

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
                    });
                }
            }
        }
    }
}