﻿using System.Data.Common;
using System.Reflection;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Data;
using Shuttle.Core.DependencyInjection;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.QueryServer;

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
                    .FromAssembly(Assembly.Load("Shuttle.ProcessManagement")).Add()
                    .AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("ProcessManagement", "Microsoft.Data.SqlClient");
                    })
                    .AddSqlSubscription()
                    .AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new AzureStorageQueueOptions
                        {
                            ConnectionString = configuration.GetConnectionString("azure")
                        });
                    })
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.Options.Subscription.ConnectionStringName = "ProcessManagement";

                        builder.AddSubscription<OrderProcessRegistered>();
                        builder.AddSubscription<OrderProcessCancelled>();
                        builder.AddSubscription<OrderProcessAccepted>();
                        builder.AddSubscription<OrderProcessCompleted>();
                        builder.AddSubscription<OrderProcessArchived>();
                        builder.AddSubscription<OrderCreated>();
                        builder.AddSubscription<CancelOrderProcessRejected>();
                        builder.AddSubscription<ArchiveOrderProcessRejected>();
                        builder.AddSubscription<InvoiceCreated>();
                        builder.AddSubscription<EMailSent>();
                    });
            })
            .Build()
            .Run();
    }
}