﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.Process;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Events.v1;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.ProcessManagement.Server;

public class OrderProcess :
    IProcessManager,
    IAsyncProcessStartMessageHandler<RegisterOrderProcess>,
    IAsyncProcessMessageHandler<CancelOrderProcess>,
    IAsyncProcessMessageHandler<AcceptOrderProcess>,
    IAsyncProcessMessageHandler<OrderCreated>,
    IAsyncProcessMessageHandler<InvoiceCreated>,
    IAsyncProcessMessageHandler<EMailSent>,
    IAsyncProcessMessageHandler<CompleteOrderProcess>,
    IAsyncProcessMessageHandler<ArchiveOrderProcess>
{
    private readonly List<ItemAdded> _items = new();
    private CustomerAssigned _customerAssigned;
    private Initialized _initialized;
    private InvoiceIdAssigned _invoiceIdAssigned;
    private OrderIdAssigned _orderIdAssigned;
    private StatusChanged _statusChanged;
    private TargetSystemAssigned _targetSystemAssigned;

    public OrderProcess()
        : this(Guid.NewGuid())
    {
    }

    public OrderProcess(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public bool CanArchive => Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase);

    public bool CanCancel => Status.Equals("Cooling Off", StringComparison.InvariantCultureIgnoreCase);

    public string Status => _statusChanged != null ? _statusChanged.Status : string.Empty;

    public decimal Total { get; private set; }

    public Guid CorrelationId { get; set; }

    public async Task ProcessMessageAsync(IProcessHandlerContext<AcceptOrderProcess> context)
    {
        if (context.Stream.IsEmpty)
        {
            return;
        }

        context.Stream.AddEvent(ChangeStatus("Order Accepted"));

        var command = new CreateOrder
        {
            OrderNumber = _initialized.OrderNumber,
            OrderDate = _initialized.DateRegistered,
            CustomerName = _customerAssigned.CustomerName,
            CustomerEMail = _customerAssigned.CustomerEMail
        };

        foreach (var itemAdded in _items)
        {
            command.Items.Add(new MessageOrderItem
            {
                Description = itemAdded.Description,
                Price = itemAdded.Price
            });
        }

        await context.SendAsync(command);

        await context.PublishAsync(new OrderProcessAccepted
        {
            OrderProcessId = CorrelationId
        });
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<ArchiveOrderProcess> context)
    {
        if (!CanArchive)
        {
            await context.PublishAsync(new ArchiveOrderProcessRejected
            {
                OrderProcessId = context.Message.OrderProcessId,
                Status = Status
            });

            return;
        }

        context.Stream.AddEvent(ChangeStatus("Order Archived"));

        await context.PublishAsync(new OrderProcessArchived
        {
            OrderProcessId = context.Message.OrderProcessId
        });
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<CancelOrderProcess> context)
    {
        if (!CanCancel)
        {
            await context.PublishAsync(new CancelOrderProcessRejected
            {
                OrderProcessId = context.Message.OrderProcessId,
                Status = Status
            });

            return;
        }

        context.Stream.Remove();

        await context.PublishAsync(new OrderProcessCancelled
        {
            OrderProcessId = context.Message.OrderProcessId
        });
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<CompleteOrderProcess> context)
    {
        context.Stream.AddEvent(ChangeStatus("Completed"));

        await context.PublishAsync(new OrderProcessCompleted
        {
            OrderProcessId = CorrelationId
        });
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<EMailSent> context)
    {
        if (!ShouldProcess(context.TransportMessage, context.Stream))
        {
            return;
        }

        context.Stream.AddEvent(ChangeStatus("Dispatched-EMail Sent"));

        await context.SendAsync(new CompleteOrderProcess
        {
            OrderProcessId = CorrelationId
        }, c => c.Local());
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<InvoiceCreated> context)
    {
        if (!ShouldProcess(context.TransportMessage, context.Stream))
        {
            return;
        }

        context.Stream.AddEvent(ChangeStatus("Invoice Created"));
        context.Stream.AddEvent(AssignInvoiceId(context.Message.InvoiceId));

        await context.SendAsync(new SendEMail
        {
            RecipientEMail = _customerAssigned.CustomerEMail,
            HtmlBody =
                $"Hello {_customerAssigned.CustomerName},<br/><br/>Your order number {_initialized.OrderNumber} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team"
        });
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<OrderCreated> context)
    {
        if (!ShouldProcess(context.TransportMessage, context.Stream))
        {
            return;
        }

        context.Stream.AddEvent(ChangeStatus("Order Created"));
        context.Stream.AddEvent(AssignOrderId(context.Message.OrderId));

        var command = new CreateInvoice
        {
            OrderId = _orderIdAssigned.OrderId,
            AccountContactName = _customerAssigned.CustomerName,
            AccountContactEMail = _customerAssigned.CustomerEMail
        };

        foreach (var item in _items)
        {
            command.Items.Add(new MessageInvoiceItem
            {
                Description = item.Description,
                Price = item.Price
            });
        }

        await context.SendAsync(command);
    }

    public async Task ProcessMessageAsync(IProcessHandlerContext<RegisterOrderProcess> context)
    {
        var message = context.Message;

        context.Stream.AddEvent(Initialize());
        context.Stream.AddEvent(AssignCustomer(message.CustomerName, message.CustomerEMail));
        context.Stream.AddEvent(AssignTargetSystem(message.TargetSystem, message.TargetSystemUri));
        context.Stream.AddEvent(ChangeStatus("Cooling Off"));

        foreach (var quotedProduct in message.QuotedProducts)
        {
            context.Stream.AddEvent(
                AddItem(quotedProduct.ProductId, quotedProduct.Description, quotedProduct.Price));
        }

        await context.PublishAsync(new OrderProcessRegistered
        {
            OrderProcessId = CorrelationId,
            QuotedProducts = message.QuotedProducts,
            CustomerName = message.CustomerName,
            CustomerEMail = message.CustomerEMail,
            OrderNumber = _initialized.OrderNumber,
            OrderDate = _initialized.DateRegistered,
            OrderTotal = Total,
            Status = _statusChanged.Status,
            StatusDate = _statusChanged.StatusDate,
            TargetSystem = message.TargetSystem,
            TargetSystemUri = message.TargetSystemUri
        });

        await context.SendAsync(new AcceptOrderProcess
        {
            OrderProcessId = CorrelationId
        }, c => c.Defer(DateTime.Now.AddSeconds(10)).Local().WithCorrelationId(CorrelationId.ToString("N")));
    }

    public ItemAdded AddItem(Guid productId, string description, decimal price)
    {
        return On(new ItemAdded
        {
            ProductId = productId,
            Description = description,
            Price = price
        });
    }

    public CustomerAssigned AssignCustomer(string customerName, string customerEMail)
    {
        return On(new CustomerAssigned
        {
            CustomerName = customerName,
            CustomerEMail = customerEMail
        });
    }

    public InvoiceIdAssigned AssignInvoiceId(Guid invoiceId)
    {
        return On(new InvoiceIdAssigned
        {
            InvoiceId = invoiceId
        });
    }

    public OrderIdAssigned AssignOrderId(Guid orderId)
    {
        return On(new OrderIdAssigned
        {
            OrderId = orderId
        });
    }

    public TargetSystemAssigned AssignTargetSystem(string targetSystem, string targetSystemUri)
    {
        return On(new TargetSystemAssigned
        {
            TargetSystem = targetSystem,
            TargetSystemUri = targetSystemUri
        });
    }

    public StatusChanged ChangeStatus(string status)
    {
        return On(new StatusChanged
        {
            Status = status,
            StatusDate = DateTime.Now
        });
    }

    private string GenerateOrderNumber(DateTime dateRegistered)
    {
        return $"ORD-{dateRegistered.Ticks.ToString().Substring(8, 6)}-{Guid.NewGuid().ToString("N")[6..]}";
    }

    public Initialized Initialize()
    {
        var now = DateTime.Now;

        return On(new Initialized
        {
            OrderNumber = GenerateOrderNumber(now),
            DateRegistered = now
        });
    }

    private Initialized On(Initialized initialized)
    {
        Guard.AgainstNull(initialized, nameof(initialized));

        return _initialized = initialized;
    }

    private CustomerAssigned On(CustomerAssigned customerAssigned)
    {
        Guard.AgainstNull(customerAssigned, nameof(customerAssigned));

        return _customerAssigned = customerAssigned;
    }

    private TargetSystemAssigned On(TargetSystemAssigned targetSystemAssigned)
    {
        Guard.AgainstNull(targetSystemAssigned, nameof(targetSystemAssigned));

        return _targetSystemAssigned = targetSystemAssigned;
    }

    private StatusChanged On(StatusChanged statusChanged)
    {
        Guard.AgainstNull(statusChanged, nameof(statusChanged));

        return _statusChanged = statusChanged;
    }

    private ItemAdded On(ItemAdded itemAdded)
    {
        Guard.AgainstNull(itemAdded, nameof(itemAdded));

        _items.Add(itemAdded);

        Total += itemAdded.Price;

        return itemAdded;
    }

    private InvoiceIdAssigned On(InvoiceIdAssigned invoiceIdAssigned)
    {
        Guard.AgainstNull(invoiceIdAssigned, nameof(invoiceIdAssigned));

        return _invoiceIdAssigned = invoiceIdAssigned;
    }

    private OrderIdAssigned On(OrderIdAssigned orderIdAssigned)
    {
        Guard.AgainstNull(orderIdAssigned, nameof(orderIdAssigned));

        return _orderIdAssigned = orderIdAssigned;
    }

    private static bool ShouldProcess(TransportMessage transportMessage, EventStream stream)
    {
        if (!transportMessage.IsHandledHere())
        {
            return false;
        }

        if (stream.IsEmpty)
        {
            throw new ApplicationException(
                $"Could not find an order process with correlation id '{transportMessage.CorrelationId}'.");
        }

        return true;
    }
}