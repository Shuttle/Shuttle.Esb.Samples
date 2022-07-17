using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shuttle.Core.WorkerService;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;

namespace Shuttle.RequestResponse.Server
{
    public class Host : IServiceHost
    {
        private IServiceBus _bus;

        public void Stop()
        {
            _bus.Dispose();
        }

        public void Start(IServiceProvider serviceProvider)
        {
            _bus = serviceProvider.GetRequiredService<IServiceBus>().Start();
        }

        public void Configure(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services.AddSingleton<IConfiguration>(configuration);

                services.AddServiceBus(serviceBus =>
                {
                    configuration.GetSection(ServiceBusOptions.SectionName).Bind(serviceBus.Options);
                });

                services.AddAzureStorageQueues(azure =>
                {
                    azure.AddConnectionString("azure");
                });
            });
        }
    }
}