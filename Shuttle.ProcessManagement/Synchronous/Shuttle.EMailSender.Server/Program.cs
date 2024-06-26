﻿using System.Data.Common;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Sql.Subscription;

namespace Shuttle.EMailSender.Server;

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
                    }).AddSqlSubscription().AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.Options.Subscription.ConnectionStringName = "ProcessManagement";
                    }).AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new AzureStorageQueueOptions
                        {
                            ConnectionString = configuration.GetConnectionString("azure")
                        });
                    });
            })
            .Build()
            .Run();
    }
}