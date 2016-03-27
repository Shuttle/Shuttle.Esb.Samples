using System;
using System.Collections.Generic;
using Shuttle.Core.Infrastructure;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Esb.Process;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Process.ESModule.v1;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.ESModule.Server
{
    public class OrderProcess :
        IProcessManager,
        IProcessStartMessageHandler<RegisterOrderProcessCommand>,
        IProcessMessageHandler<CancelOrderProcessCommand>,
        IProcessMessageHandler<AcceptOrderProcessCommand>,
        IProcessMessageHandler<OrderCreatedEvent>,
        IProcessMessageHandler<InvoiceCreatedEvent>,
        IProcessMessageHandler<EMailSentEvent>,
        IProcessMessageHandler<CompleteOrderProcessCommand>,
        IProcessMessageHandler<ArchiveOrderProcessCommand>
    {
        private readonly List<ItemAdded> _items = new List<ItemAdded>();
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

        public decimal Total { get; private set; }

        public string Status
        {
            get { return _statusChanged != null ? _statusChanged.Status : string.Empty; }
        }

        public bool CanArchive
        {
            get { return Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool CanCancel
        {
            get { return Status.Equals("Cooling Off", StringComparison.InvariantCultureIgnoreCase); }
        }

        public Guid CorrelationId { get; set; }

        public void ProcessMessage(IProcessHandlerContext<AcceptOrderProcessCommand> context)
        {
            if (context.Stream.IsEmpty)
            {
                return;
            }

            context.Stream.AddEvent(ChangeStatus("Order Accepted"));

            var command = new CreateOrderCommand
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

            context.Send(command);

            context.Publish(new OrderProcessAcceptedEvent
            {
                OrderProcessId = CorrelationId
            });
        }

        public void ProcessMessage(IProcessHandlerContext<CancelOrderProcessCommand> context)
        {
            if (!CanCancel)
            {
                context.Publish(new CancelOrderProcessRejectedEvent
                {
                    OrderProcessId = context.Message.OrderProcessId,
                    Status = Status
                });

                return;
            }

            context.Stream.Remove();

            context.Publish(new OrderProcessCancelledEvent
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }

        public void ProcessMessage(IProcessHandlerContext<CompleteOrderProcessCommand> context)
        {
            context.Stream.AddEvent(ChangeStatus("Completed"));

            context.Publish(new OrderProcessCompletedEvent
            {
                OrderProcessId = CorrelationId
            });
        }

        public void ProcessMessage(IProcessHandlerContext<EMailSentEvent> context)
        {
            if (!ShouldProcess(context.TransportMessage, context.Stream))
            {
                return;
            }

            context.Stream.AddEvent(ChangeStatus("Dispatched-EMail Sent"));

            context.Send(new CompleteOrderProcessCommand
            {
                OrderProcessId = CorrelationId
            }, c => c.Local());
        }

        public void ProcessMessage(IProcessHandlerContext<InvoiceCreatedEvent> context)
        {
            if (!ShouldProcess(context.TransportMessage, context.Stream))
            {
                return;
            }

            context.Stream.AddEvent(ChangeStatus("Invoice Created"));
            context.Stream.AddEvent(AssignInvoiceId(context.Message.InvoiceId));

            context.Send(new SendEMailCommand
            {
                RecipientEMail = _customerAssigned.CustomerEMail,
                HtmlBody =
                    string.Format(
                        "Hello {0},<br/><br/>Your order number {1} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team",
                        _customerAssigned.CustomerName, _initialized.OrderNumber)
            });
        }

        public void ProcessMessage(IProcessHandlerContext<OrderCreatedEvent> context)
        {
            if (!ShouldProcess(context.TransportMessage, context.Stream))
            {
                return;
            }

            context.Stream.AddEvent(ChangeStatus("Order Created"));
            context.Stream.AddEvent(AssignOrderId(context.Message.OrderId));

            var command = new CreateInvoiceCommand
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

            context.Send(command);
        }

        public void ProcessMessage(IProcessHandlerContext<RegisterOrderProcessCommand> context)
        {
            var message = context.Message;

            context.Stream.AddEvent(Initialize());
            context.Stream.AddEvent(AssignCustomer(message.CustomerName, message.CustomerEMail));
            context.Stream.AddEvent(AssignTargetSystem(message.TargetSystem, message.TargetSystemUri));
            context.Stream.AddEvent(ChangeStatus("Cooling Off"));

            foreach (var quotedProduct in message.QuotedProducts)
            {
                context.Stream.AddEvent(AddItem(quotedProduct.ProductId, quotedProduct.Description, quotedProduct.Price));
            }

            context.Publish(new OrderProcessRegisteredEvent
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

            context.Send(new AcceptOrderProcessCommand
            {
                OrderProcessId = CorrelationId
            }, c => c.Defer(DateTime.Now.AddSeconds(10)).Local().WithCorrelationId(CorrelationId.ToString("N")));
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

        public Initialized On(Initialized initialized)
        {
            Guard.AgainstNull(initialized, "initialized");

            return _initialized = initialized;
        }

        private string GenerateOrderNumber(DateTime dateRegistered)
        {
            return string.Format("ORD-{0}-{1}",
                dateRegistered.Ticks.ToString().Substring(8, 6),
                Guid.NewGuid().ToString("N").Substring(6));
        }

        public CustomerAssigned AssignCustomer(string customerName, string customerEMail)
        {
            return On(new CustomerAssigned
            {
                CustomerName = customerName,
                CustomerEMail = customerEMail
            });
        }

        public CustomerAssigned On(CustomerAssigned customerAssigned)
        {
            Guard.AgainstNull(customerAssigned, "customerAssigned");

            return _customerAssigned = customerAssigned;
        }

        public TargetSystemAssigned AssignTargetSystem(string targetSystem, string targetSystemUri)
        {
            return On(new TargetSystemAssigned
            {
                TargetSystem = targetSystem,
                TargetSystemUri = targetSystemUri
            });
        }

        public TargetSystemAssigned On(TargetSystemAssigned targetSystemAssigned)
        {
            Guard.AgainstNull(targetSystemAssigned, "targetSystemAssigned");

            return _targetSystemAssigned = targetSystemAssigned;
        }

        public StatusChanged ChangeStatus(string status)
        {
            return On(new StatusChanged
            {
                Status = status,
                StatusDate = DateTime.Now
            });
        }

        public StatusChanged On(StatusChanged statusChanged)
        {
            Guard.AgainstNull(statusChanged, "statusChanged");

            return _statusChanged = statusChanged;
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

        public ItemAdded On(ItemAdded itemAdded)
        {
            Guard.AgainstNull(itemAdded, "itemAdded");

            _items.Add(itemAdded);

            Total += itemAdded.Price;

            return itemAdded;
        }

        public InvoiceIdAssigned AssignInvoiceId(Guid invoiceId)
        {
            return On(new InvoiceIdAssigned
            {
                InvoiceId = invoiceId
            });
        }

        public InvoiceIdAssigned On(InvoiceIdAssigned invoiceIdAssigned)
        {
            Guard.AgainstNull(invoiceIdAssigned, "invoiceIdAssigned");

            return _invoiceIdAssigned = invoiceIdAssigned;
        }

        public OrderIdAssigned AssignOrderId(Guid orderId)
        {
            return On(new OrderIdAssigned
            {
                OrderId = orderId
            });
        }

        public OrderIdAssigned On(OrderIdAssigned orderIdAssigned)
        {
            Guard.AgainstNull(orderIdAssigned, "orderIdAssigned");

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
                    string.Format("Could not find an order process with correlation id '{0}'.",
                        transportMessage.CorrelationId));
            }

            return true;
        }

        public void ProcessMessage(IProcessHandlerContext<ArchiveOrderProcessCommand> context)
        {
            if (!CanArchive)
            {
                context.Publish(new ArchiveOrderProcessRejectedEvent
                {
                    OrderProcessId = context.Message.OrderProcessId,
                    Status = Status
                });

                return;
            }

            context.Stream.AddEvent(ChangeStatus("Order Archived"));

            context.Publish(new OrderProcessArchivedEvent
            {
                OrderProcessId = context.Message.OrderProcessId
            });
        }
    }
}