using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddServiceBus(builder =>
            {
                configuration.GetSection(ServiceBusOptions.SectionName)
                    .Bind(builder.Options);
            })
            .AddAzureStorageQueues(builder =>
            {
                builder.AddOptions("azure", new()
                {
                    ConnectionString = "UseDevelopmentStorage=true;"
                });
            });

        Console.WriteLine("Type some characters and then press [enter] to submit; an empty line submission stops execution:");
        Console.WriteLine();

        await using (var serviceBus = await services.BuildServiceProvider()
                         .GetRequiredService<IServiceBus>().StartAsync())
        {
            string? userName;

            while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
            {
                await serviceBus.SendAsync(new RegisterMember
                {
                    UserName = userName
                }, c => c.WillExpire(DateTime.Now.AddSeconds(5)));
            }
        }
    }
}