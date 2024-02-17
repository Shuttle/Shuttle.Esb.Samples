using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Esb;
using Shuttle.Esb.Kafka;
using Shuttle.StreamProcessing.Messages;

namespace Shuttle.StreamProcessing.Producer
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

            services.AddKafka(builder =>
            {
                builder.AddOptions("local", new KafkaOptions
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

            using (var serviceBus = services.BuildServiceProvider().GetRequiredService<IServiceBus>().Start())
            {
                string name;

                while (!string.IsNullOrEmpty(name = Console.ReadLine()))
                {
                    for (var minute = 0; minute < 1440; minute++)
                    {
                        serviceBus.Send(new TemperatureRead
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
}