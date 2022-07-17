using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.Deferred.Server
{
    public class Programs
    {
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                    services.AddSingleton<IConfiguration>(configuration);

                    services.AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);
                    });

                    services.AddAzureStorageQueues(builder =>
                    {
                        builder.AddConnectionString("azure");
                    });
                })
                .Build();

            var serviceBus = host.Services.GetRequiredService<IServiceBus>().Start();

            host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() =>
            {
                serviceBus.Dispose();
            });

            host.Run();
        }
    }
}