using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;

namespace Shuttle.Process.Custom.Server;

public class OrderCreatedHandler : IAsyncMessageHandler<OrderCreated>
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IOrderProcessRepository _repository;

    public OrderCreatedHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
    {
        Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        Guard.AgainstNull(repository, nameof(repository));

        _databaseContextFactory = databaseContextFactory;
        _repository = repository;
    }

    public async Task ProcessMessageAsync(IHandlerContext<OrderCreated> context)
    {
        if (!context.TransportMessage.IsHandledHere())
        {
            return;
        }

        OrderProcess orderProcess;

        using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
        {
            orderProcess = await _repository.GetAsync(new Guid(context.TransportMessage.CorrelationId));

            if (orderProcess == null)
            {
                throw new ApplicationException($"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
            }

            var orderProcessStatus = new OrderProcessStatus("Order Created");

            orderProcess.AddStatus(orderProcessStatus);

            await _repository.SaveOrderIdAsync(context.Message.OrderId, orderProcess.Id);
            await _repository.AddStatusAsync(orderProcessStatus, orderProcess.Id);
        }

        var createInvoiceCommand = new CreateInvoice
        {
            OrderId = context.Message.OrderId,
            AccountContactName = orderProcess.CustomerName,
            AccountContactEMail = orderProcess.CustomerEMail
        };

        foreach (var messageOrderItem in context.Message.Items)
        {
            createInvoiceCommand.Items.Add(new MessageInvoiceItem
            {
                Description = messageOrderItem.Description,
                Price = messageOrderItem.Price
            });
        }

        await context.SendAsync(createInvoiceCommand, c => c.WithCorrelationId(orderProcess.Id.ToString()));
    }
}