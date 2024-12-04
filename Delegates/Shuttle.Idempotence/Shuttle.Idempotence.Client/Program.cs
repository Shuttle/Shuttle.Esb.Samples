using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddServiceBus(builder =>
            {
                configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);
            })
            .AddAzureStorageQueues(builder =>
            {
                builder.AddOptions("azure", new()
                {
                    ConnectionString = Guard.AgainstNullOrEmptyString(configuration.GetConnectionString("azure"))
                });
            });

        Console.WriteLine("Type some characters and then press [enter] to submit; an empty line submission stops execution:");
        Console.WriteLine();

        var serviceProvider = services.BuildServiceProvider();
        var pipelineFactory = serviceProvider.GetRequiredService<IPipelineFactory>();
        var messageSender = serviceProvider.GetRequiredService<IMessageSender>();
        var transportMessagePipeline = pipelineFactory.GetPipeline<TransportMessagePipeline>();

        await using (var serviceBus = await serviceProvider.GetRequiredService<IServiceBus>().StartAsync())
        {
            string userName;

            while (!string.IsNullOrEmpty(userName = Console.ReadLine() ?? string.Empty))
            {
                var command = new RegisterMember
                {
                    UserName = userName
                };

                await transportMessagePipeline.ExecuteAsync(command, null, null);

                var transportMessage = Guard.AgainstNull(transportMessagePipeline.State.GetTransportMessage());

                for (var i = 0; i < 5; i++)
                {
                    await messageSender.DispatchAsync(transportMessage); // will be processed only once since message id is the same
                }

                await serviceBus.SendAsync(command); // will be processed since it has a new message id
                await serviceBus.SendAsync(command); // will also be processed since it too has a new message id
            }
        }
    }
}