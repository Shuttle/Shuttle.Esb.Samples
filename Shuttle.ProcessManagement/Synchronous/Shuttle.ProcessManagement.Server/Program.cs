using System.Data.Common;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Process;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Recall;
using Shuttle.Recall.Sql.Storage;

namespace Shuttle.Process.ESModule.Server;

public class Program
{
    public static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("ProcessManagement", "Microsoft.Data.SqlClient");
                    })
                    .AddSqlSubscription()
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.Options.Subscription.ConnectionStringName = "ProcessManagement";

                        builder.AddSubscription<OrderCreated>();
                        builder.AddSubscription<InvoiceCreated>();
                        builder.AddSubscription<EMailSent>();
                    })
                    .AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new AzureStorageQueueOptions
                        {
                            ConnectionString = configuration.GetConnectionString("azure")
                        });
                    })
                    .AddSqlEventStorage(builder =>
                    {
                        builder.Options.ConnectionStringName = "ProcessManagement";
                    })
                    .AddEventStore()
                    .AddProcessManagement();
            })
            .Build()
            .Run();
    }
}