using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Esb;
using Shuttle.Esb.Kafka;
using Shuttle.StreamProcessing.Messages;

namespace Shuttle.StreamProcessing.Consumer;

internal class Program
{
    private static async Task Main()
    {
        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.AddMessageHandler(async (IHandlerContext<TemperatureRead> context) =>
                        {
                            Console.WriteLine($"[TEMPERATURE READ] : name = '{context.Message.Name}' / minute = {context.Message.Minute} / celsius = {context.Message.Celsius:F}");

                            await Task.CompletedTask;
                        });
                    })
                    .AddKafka(builder =>
                    {
                        builder.AddOptions("local", new()
                        {
                            BootstrapServers = "localhost:9092",
                            EnableAutoCommit = true,
                            EnableAutoOffsetStore = true,
                            NumPartitions = 1,
                            UseCancellationToken = false,
                            ConsumeTimeout = TimeSpan.FromMilliseconds(25)
                        });
                    });
            })
            .Build()
            .RunAsync();
    }
}