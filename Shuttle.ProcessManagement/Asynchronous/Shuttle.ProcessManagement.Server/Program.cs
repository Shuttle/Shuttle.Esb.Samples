using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Data.Logging;
using Shuttle.Core.Json;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Logging;
using Shuttle.Esb.Process;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Logging;
using Shuttle.Recall;
using Shuttle.Recall.Logging;
using Shuttle.Recall.Sql.Storage;

namespace Shuttle.ProcessManagement.Server;

public class Program
{
    private static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);


        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(new FileLoggerProvider("Shuttle.ProcessManagement.Server")));

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddJsonSerializer()
                    .AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("ProcessManagement", "Microsoft.Data.SqlClient");
                    })
                    .AddSqlSubscription()
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.Options.Subscription.ConnectionStringName = "ProcessManagement";
                        builder.Options.Asynchronous = true;

                        builder.AddSubscription<OrderCreated>();
                        builder.AddSubscription<InvoiceCreated>();
                        builder.AddSubscription<EMailSent>();
                    })
                    .AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new AzureStorageQueueOptions
                        {
                            ConnectionString = configuration.GetConnectionString("azure"),
                            VisibilityTimeout = TimeSpan.FromMinutes(5)
                        });
                    })
                    .AddSqlEventStorage(builder =>
                    {
                        builder.Options.ConnectionStringName = "ProcessManagement";
                    })
                    .AddEventStore()
                    .AddProcessManagement()
                    .AddLogging()
                    .AddDataAccessLogging()
                    .AddServiceBusLogging(builder =>
                    {
                        builder.Options.AddPipelineEventType<OnHandleMessage>();
                        builder.Options.AddPipelineEventType<OnAcknowledgeMessage>();
                    })
                    .AddRecallLogging(builder =>
                    {
                        builder.Options.AddPipelineEventType<OnGetStreamEvents>();
                        builder.Options.AddPipelineEventType<OnSavePrimitiveEvents>();
                        builder.Options.AddPipelineEventType<OnRemoveEventStream>();
                        builder.Options.AddPipelineEventType<OnAcknowledgeEvent>();
                    });
            })
            .Build()
            .RunAsync();
    }
}