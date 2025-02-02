# Idempotence

This sample makes use of [Shuttle.Esb.AzureStorageQueues](https://github.com/Shuttle/Shuttle.Esb.AzureStorageQueues) for the message queues.  Local Azure Storage Queues should be provided by [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio).

Once you have opened the `Shuttle.Idempotence.sln` solution in Visual Studio set the following projects as startup projects:

- Shuttle.Idempotence.Client
- Shuttle.Idempotence.Server

You will also need to create and configure a Sql Server database for this sample and remember to update the **App.config** `connectionString` settings to point to your database.  Please reference the **Database** section below.

## Implementation

When operations, or in our case messages, can be applied multiple times with the same result they are said to be **idempotent**.  Idempotence is something you should strive to implement directly on your endpoint by keeping track of some unique property of each message and whether the operation has been completed for that unique property.

An `IIdempotenceService` implementation can assist with this from a technical point-of-view by allowing a particular message id to be handled only once.  This works fine for our ***at-least-once*** delivery mechanism where, in some edge case, we may receive the same message again.  However, it will not aid us when two messages are going to be sent, each with its own message id, but they contain the same data.

In this guide we'll create the following projects:

- `Shuttle.Idempotence.Client` (**Console Application**)
- `Shuttle.Idempotence.Server` (**Console Application**)
- `Shuttle.Idempotence.Messages` (**Class Library**)

## Messages

> Create a new class library called `Shuttle.Idempotence.Messages` with a solution called `Shuttle.Idempotence`

**Note**: remember to change the *Solution name*.

### RegisterMember

> Rename the `Class1` default file to `RegisterMember` and add a `UserName` property.

``` c#
namespace Shuttle.Idempotence.Messages;

public class RegisterMember
{
    public string UserName { get; set; } = string.Empty;
}
```

## Client

> Add a new `Console Application` to the solution called `Shuttle.Idempotence.Client`.

> Install the `Shuttle.Esb.AzureStorageQueues` nuget package.

This will provide access to the Azure Storage Queues `IQueue` implementation and also include the required dependencies.

> Install the `Microsoft.Extensions.Configuration.Json` nuget package.

This will provide the ability to read the `appsettings.json` file.

> Add a reference to the `Shuttle.Idempotence.Messages` project.

### Program

> Implement the main client code as follows:

``` c#
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddServiceBus(builder =>
            {
                configuration.GetSection(ServiceBusOptions.SectionName)
                    .Bind(builder.Options);
            })
            .AddAzureStorageQueues(builder =>
            {
                builder.AddOptions("azure", new()
                {
                    ConnectionString = Guard.AgainstNullOrEmptyString(configuration.GetConnectionString("azure"))
                });
            });

        Console.WriteLine("Type some characters and then press [enter] to submit; an empty line submission stops execution:");
        Console.WriteLine();

        var serviceProvider = services.BuildServiceProvider();
        var pipelineFactory = serviceProvider.GetRequiredService<IPipelineFactory>();
        var messageSender = serviceProvider.GetRequiredService<IMessageSender>();
        var transportMessagePipeline = pipelineFactory.GetPipeline<TransportMessagePipeline>();

        await using (var serviceBus = await serviceProvider.GetRequiredService<IServiceBus>().StartAsync())
        {
            string userName;

            while (!string.IsNullOrEmpty(userName = Console.ReadLine() ?? string.Empty))
            {
                var command = new RegisterMember
                {
                    UserName = userName
                };

                await transportMessagePipeline.ExecuteAsync(command, null, null);

                var transportMessage = Guard.AgainstNull(transportMessagePipeline.State.GetTransportMessage());

                for (var i = 0; i < 5; i++)
                {
                    await messageSender.DispatchAsync(transportMessage); // will be processed only once since message id is the same
                }

                await serviceBus.SendAsync(command); // will be processed since it has a new message id
                await serviceBus.SendAsync(command); // will also be processed since it too has a new message id
            }
        }
    }
}
```

Keep in mind that the when you `Send` a message a `TransportMessage` envelope is created with a unique message id (`Guid`).  In the above code we first manually create a `TransportMessage` so that we can send technically identical messages (with the same message id).

The next two `Send` operations do not use the `TransportMessage` but rather send individual messages.  These will each have a `TransportMessage` envelope and, therefore, each have its own unique message id.

### Client configuration file

> Add an `appsettings.json` file as follows:

```json
{
  "ConnectionStrings": {
    "azure": "UseDevelopmentStorage=true;"
  },
  "Shuttle": {
    "ServiceBus": {
      "MessageRoutes": [
        {
          "Uri": "azuresq://azure/shuttle-server-work",
          "Specifications": [
            {
              "Name": "StartsWith",
              "Value": "Shuttle.Idempotence.Messages"
            }
          ]
        }
      ]
    }
  }
}
```

This tells the service bus that all messages sent having a type name starting with `Shuttle.Idempotence.Messages` should be sent to endpoint `azuresq://azure/shuttle-server-work`.

## Server

> Add a new `Console Application` to the solution called `Shuttle.Idempotence.Server`.

> Install the `Shuttle.Esb.AzureStorageQueues` nuget package.

This will provide access to the Azure Storage Queues `IQueue` implementation and also include the required dependencies.

> Install the `Microsoft.Extensions.Hosting` nuget package.

This allows a console application to be hosted using the .NET generic host.

> Install the `Microsoft.Extensions.Configuration.Json` nuget package.

This will provide the ability to read the `appsettings.json` file.

> Install the `Shuttle.Esb.Sql.Idempotence` package. 

We will also have access to the Sql implementation of the `IIdempotenceService`.

> Install the `Microsoft.Data.SqlClient` nuget package.

This will provide a connection to our Sql Server.

> Add a reference to the `Shuttle.Idempotence.Messages` project.

### Program

Implement the `Program` class as follows:

``` c#
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
using Shuttle.Esb.Idempotence;
using Shuttle.Esb.Sql.Idempotence;

namespace Shuttle.Idempotence.Server;

public class Program
{
    public static async Task Main()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json").Build();

                services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("Idempotence", "Microsoft.Data.SqlClient");
                    })
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName)
                            .Bind(builder.Options);
                    })
                    .AddIdempotence()
                    .AddSqlIdempotence(builder =>
                    {
                        builder.Options.ConnectionStringName = "Idempotence";

                        builder.UseSqlServer();
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
```

### Database

We need a store for our idempotence tracking.  In this example we will be using **Sql Server**.  If you are using docker you can quickly get up-and-running with the following:

```
docker run -d --name sql -h sql -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Pass!000" -e "MSSQL_PID=Express" -p 1433:1433 -v C:\SQLServer.Data\:/var/opt/mssql/data mcr.microsoft.com/mssql/server:2019-latest
```

> Create a new database called **Shuttle**

The implementation will create any required database structures on startup.  If you need to execute the creation scripts manually, please reference the [source code](https://github.com/Shuttle/Shuttle.Esb.Sql.Idempotence).

### Server configuration file

> Add an `appsettings.json` file as follows:

```json
{
  "ConnectionStrings": {
    "azure": "UseDevelopmentStorage=true;",
    "Idempotence": "server=.;database=shuttle;user id=sa;password=Pass!000;TrustServerCertificate=True"
  },
  "Shuttle": {
    "ServiceBus": {
      "Inbox": {
        "WorkQueueUri": "azuresq://azure/shuttle-server-work",
        "DeferredQueueUri": "azuresq://azure/shuttle-server-deferre",
        "ErrorQueueUri": "azuresq://azure/shuttle-error"
      }
    }
  }
}
```

### RegisterMemberHandler

> Add a new class called `RegisterMemberHandler` that implements the `IMessageHandler<RegisterMember>` interface as follows:

``` c#
using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Server;

public class RegisterMemberHandler : IMessageHandler<RegisterMember>
{
    public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
    {
        Console.WriteLine();
        Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}' / message id = '{context.TransportMessage.MessageId}'");
        Console.WriteLine();

        await Task.CompletedTask;
    }
}
```

This will write out some information to the console window.

## Run

> Set both the client and server projects as startup projects.

### Execute

> Execute the application.

> The **client** application will wait for you to input a user name.  For this example enter **my user name** and press enter:

::: info
You will need to scroll through the messages but you will observe that the server application has processed all three messages.
:::

You have now implemented message idempotence.
