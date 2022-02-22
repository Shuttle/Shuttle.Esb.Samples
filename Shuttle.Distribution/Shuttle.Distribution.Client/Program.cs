using System;
using Shuttle.Core.Container;
using Shuttle.Core.Unity;
using Shuttle.Distribution.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Unity;

namespace Shuttle.Distribution.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new UnityComponentContainer(new UnityContainer());

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