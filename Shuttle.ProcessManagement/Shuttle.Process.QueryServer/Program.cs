using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer
{
    public class Program
    {
        public static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                    services.AddSingleton<IConfiguration>(configuration);

                    services.AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("ProcessManagement", "System.Data.SqlClient");
                    });

                    services.AddSqlSubscription();

                    services.AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new AzureStorageQueueOptions
                        {
                            ConnectionString = configuration.GetConnectionString("azure")
                        });
                    });

                    services.AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.Options.Subscription.ConnectionStringName = "ProcessManagement";

                        builder.AddSubscription<OrderProcessRegisteredEvent>();
                        builder.AddSubscription<OrderProcessCancelledEvent>();
                        builder.AddSubscription<OrderProcessAcceptedEvent>();
                        builder.AddSubscription<OrderProcessCompletedEvent>();
                        builder.AddSubscription<OrderProcessArchivedEvent>();
                        builder.AddSubscription<OrderCreatedEvent>();
                        builder.AddSubscription<CancelOrderProcessRejectedEvent>();
                        builder.AddSubscription<ArchiveOrderProcessRejectedEvent>();
                        builder.AddSubscription<InvoiceCreatedEvent>();
                        builder.AddSubscription<EMailSentEvent>();
                    });
                })
                .Build()
                .Run();
        }
    }
}