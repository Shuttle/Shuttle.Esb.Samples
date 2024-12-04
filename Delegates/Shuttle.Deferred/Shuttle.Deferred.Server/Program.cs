using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Deferred.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;

namespace Shuttle.Deferred.Server;

public class Programs
{
    public static async Task Main()
    {
        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddHostedService<DeferredHostedService>()
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.AddMessageHandler(async (IHandlerContext<RegisterMember> context) =>
                        {
                            Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");

                            await Task.CompletedTask;
                        });
                    })
                    .AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new()
                        {
                            ConnectionString = configuration.GetConnectionString("azure")!
                        });
                    });
            })
            .Build()
            .RunAsync();
    }
}