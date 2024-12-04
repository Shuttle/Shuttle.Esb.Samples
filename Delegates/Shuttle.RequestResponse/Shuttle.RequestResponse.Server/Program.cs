using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Server;

internal class Program
{
    private static async Task Main()
    {
        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

                        builder.AddMessageHandler(async (IHandlerContext<RegisterMember> context) =>
                        {
                            Console.WriteLine();
                            Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}'", context.Message.UserName);
                            Console.WriteLine();

                            await context.SendAsync(new MemberRegistered
                            {
                                UserName = context.Message.UserName
                            }, transportMessageBuilder => transportMessageBuilder.Reply());
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