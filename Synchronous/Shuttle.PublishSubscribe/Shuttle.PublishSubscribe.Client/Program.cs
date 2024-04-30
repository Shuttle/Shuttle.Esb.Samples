using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddServiceBus(builder =>
            {
                configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);
            });

            services.AddAzureStorageQueues(builder =>
            {
                builder.AddOptions("azure", new AzureStorageQueueOptions
                {
                    ConnectionString = "UseDevelopmentStorage=true;"
                });
            });

            Console.WriteLine("Type some characters and then press [enter] to submit; an empty line submission stops execution:");
            Console.WriteLine();

            using (var serviceBus = services.BuildServiceProvider().GetRequiredService<IServiceBus>().Start())
            {
                string userName;

                while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
                {
                    serviceBus.Send(new RegisterMember
                    {
                        UserName = userName
                    });
                }
            }
        }
    }
}