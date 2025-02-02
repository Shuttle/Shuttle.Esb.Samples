using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Esb;
using Shuttle.Esb.Kafka;
using Shuttle.StreamProcessing.Messages;

namespace Shuttle.StreamProcessing.Producer;

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

        Console.WriteLine("Type a name for the set of readings, then press [enter] to submit; an empty line submission stops execution:");
        Console.WriteLine();

        var random = new Random();
        decimal temperature = random.Next(-5, 30);

        await using (var serviceBus = await services.BuildServiceProvider()
                         .GetRequiredService<IServiceBus>().StartAsync())
        {
            string name;

            while (!string.IsNullOrEmpty(name = Console.ReadLine() ?? string.Empty))
            {
                for (var minute = 0; minute < 1440; minute++)
                {
                    await serviceBus.SendAsync(new TemperatureRead
                    {
                        Name = name,
                        Minute = minute,
                        Celsius = temperature
                    });

                    if (temperature > -5 && (random.Next(0, 100) < 50 || temperature > 29))
                    {
                        temperature -= random.Next(0, 100) / 100m;
                    }
                    else
                    {
                        temperature += random.Next(0, 100) / 100m;
                    }
                }
            }
        }
    }
}