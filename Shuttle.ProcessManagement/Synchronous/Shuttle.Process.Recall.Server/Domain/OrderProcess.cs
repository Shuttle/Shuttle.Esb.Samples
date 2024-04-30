using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.EMailSender.Messages;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Events.v1;

namespace Shuttle.Process.Recall.Server.Domain;

public class OrderProcess
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

    public OrderProcess(Guid id)
    {
        Id = id;
    }

    public bool CanArchive => Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase);

    public bool CanCancel => Status.Equals("Cooling Off", StringComparison.InvariantCultureIgnoreCase);

    public Guid Id { get; }

    public string Status => _statusChanged != null ? _statusChanged.Status : string.Empty;

    public decimal Total { get; private set; }

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

    public CreateInvoice CreateInvoiceCommand()
    {
        var result = new CreateInvoice
        {
            OrderId = _orderIdAssigned.OrderId,
            AccountContactName = _customerAssigned.CustomerName,
            AccountContactEMail = _customerAssigned.CustomerEMail
        };

        foreach (var item in _items)
        {
            result.Items.Add(new MessageInvoiceItem
            {
                Description = item.Description,
                Price = item.Price
            });
        }

        return result;
    }

    public CreateOrder CreateOrderCommand()
    {
        var result = new CreateOrder
        {
            OrderNumber = _initialized.OrderNumber,
            OrderDate = _initialized.DateRegistered,
            CustomerName = _customerAssigned.CustomerName,
            CustomerEMail = _customerAssigned.CustomerEMail
        };

        foreach (var itemAdded in _items)
        {
            result.Items.Add(new MessageOrderItem
            {
                Description = itemAdded.Description,
                Price = itemAdded.Price
            });
        }

        return result;
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

    public SendEMail SendEMailCommand()
    {
        return new SendEMail
        {
            RecipientEMail = _customerAssigned.CustomerEMail,
            HtmlBody =
                string.Format(
                    "Hello {0},<br/><br/>Your order number {1} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team",
                    _customerAssigned.CustomerName, _initialized.OrderNumber)
        };
    }
}