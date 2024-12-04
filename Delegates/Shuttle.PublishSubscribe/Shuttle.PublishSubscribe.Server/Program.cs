using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Sql.Subscription;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Server;

public class Program
{
    public static async Task Main()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("Subscription", "Microsoft.Data.SqlClient");
                    })
                    .AddSqlSubscription(builder =>
                    {
                        builder.Options.ConnectionStringName = "Subscription";

                        builder.UseSqlServer();
                    })
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.AddMessageHandler(async (IHandlerContext<RegisterMember> context) =>
                        {
                            Console.WriteLine();
                            Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");
                            Console.WriteLine();

                            await context.PublishAsync(new MemberRegistered
                            {
                                UserName = context.Message.UserName
                            });
                        });
                    })
                    .AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new()
                        {
                            ConnectionString = Guard.AgainstNullOrEmptyString(configuration.GetConnectionString("azure"))
                        });
                    });
            })
            .Build()
            .RunAsync();
    }
}