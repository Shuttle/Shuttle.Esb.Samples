using System.Data.Common;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.DependencyInjection;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.ProcessManagement.DataAccess.Product;
using Shuttle.ProcessManagement.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.ProcessManagement.Services;
using Shuttle.ProcessManagement.WebApi;

DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCors()
    .FromAssembly(Assembly.Load("Shuttle.ProcessManagement")).Add()
    .AddDataAccess(builder =>
    {
        builder.AddConnectionString("ProcessManagement", "Microsoft.Data.SqlClient", "server=.;database=ProcessManagement;user id=sa;password=Pass!000;TrustServerCertificate=true");
    })
    .AddSingleton<IOrderProcessService, OrderProcessService>()
    .AddAzureStorageQueues(builder =>
    {
        builder.AddOptions("azure", new AzureStorageQueueOptions
        {
            ConnectionString = "UseDevelopmentStorage=true;"
        });
    })
    .AddServiceBus();

var app = webApplicationBuilder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapGet("/orders", (IOrderProcessService orderProcessService) =>
{
    return Guard.AgainstNull(orderProcessService, nameof(orderProcessService)).ActiveOrders();
});

app.MapDelete("/orders/{id:guid}", (IOrderProcessService orderProcessService, Guid id) =>
{
    Guard.AgainstNull(orderProcessService, nameof(orderProcessService)).CancelOrder(id);
});

app.MapDelete("/orders/{id:guid}/archive", (IOrderProcessService orderProcessService, Guid id) =>
{
    Guard.AgainstNull(orderProcessService, nameof(orderProcessService)).ArchiveOrder(id);
});

app.MapPost("/orders", (IServiceBus serviceBus, IProductQuery productQuery, RegisterOrderModel model) => {
    Guard.AgainstNull(serviceBus, nameof(serviceBus));
    Guard.AgainstNull(productQuery, nameof(productQuery));
    Guard.AgainstNull(model, nameof(model));
    Guard.AgainstNull(model.TargetSystem, nameof(model.TargetSystem));
    Guard.Against<Exception>(model.ProductIds?.Count == 0, "No products have been selected.");

    var message = new RegisterOrderProcess
    {
        CustomerName = model.CustomerName,
        CustomerEMail = model.CustomerEMail,
        TargetSystem = model.TargetSystem
    };

    switch ((model.TargetSystem ?? string.Empty).ToUpperInvariant())
    {
        case "CUSTOM":
            {
                message.TargetSystemUri = "azuresq://azure/process-server";

                break;
            }
        case "EVENT-SOURCE":
            {
                message.TargetSystemUri = "azuresq://azure/process-recall-server";

                break;
            }
        case "PROCESS-MANAGEMENT":
            {
                message.TargetSystemUri = "azuresq://azure/process-management-server";

                break;
            }
        default:
            {
                throw new ApplicationException($"Unknown target system '{model.TargetSystem}'.");
            }
    }

    foreach (var productIdValue in model.ProductIds)
    {
        if (!Guid.TryParse(productIdValue, out var productId))
        {
            throw new ArgumentException($"Product id '{productIdValue}' is not a valid guid.");
        }

        var productRow = productQuery.Get(productId);

        message.QuotedProducts.Add(new QuotedProduct
        {
            ProductId = productId,
            Description = ProductColumns.Description.Value(productRow),
            Price = ProductColumns.Price.Value(productRow)
        });
    }

    serviceBus.Send(message, builder =>
    {
        builder.WithRecipient(message.TargetSystemUri);
        builder.Headers.Add(new TransportHeader
        {
            Key = "TargetSystem",
            Value = message.TargetSystem
        });
    });
});

app.MapGet("/products", (IProductQuery productQuery) =>
{
    return Guard.AgainstNull(productQuery, nameof(productQuery)).All()
        .Select(row => new
        {
            Id = ProductColumns.Id.Value(row),
            Description = ProductColumns.Description.Value(row),
            Price = ProductColumns.Price.Value(row),
            Url = ProductColumns.Url.Value(row)
        });
});

app.Run();