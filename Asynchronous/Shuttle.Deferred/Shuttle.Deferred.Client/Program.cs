﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Deferred.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;

namespace Shuttle.Deferred.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddServiceBus(builder =>
            {
                configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                builder.Options.Asynchronous = true;
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

            await using (var serviceBus = await services.BuildServiceProvider().GetRequiredService<IServiceBus>().StartAsync())
            {
                string userName;

                while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
                {
                    await serviceBus.SendAsync(new RegisterMember
                    {
                        UserName = userName
                    }, builder => builder.Defer(DateTime.Now.AddSeconds(5)));
                }
            }
        }
    }
}