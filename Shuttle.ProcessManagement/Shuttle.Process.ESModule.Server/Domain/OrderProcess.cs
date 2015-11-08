using System;
using System.Collections.Generic;
using Shuttle.Core.Infrastructure;
using Shuttle.EMailSender.Messages;
using Shuttle.ESB.Core;
using Shuttle.ESB.Process;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall.Core;

namespace Shuttle.Process.ESModule.Server.Domain
{
    public class OrderProcess :
        IProcessManager,
        IProcessMessageHandler<AcceptOrderProcessCommand>,
        IProcessMessageHandler<OrderCreatedEvent>,
        IProcessMessageHandler<InvoiceCreatedEvent>,
        IProcessMessageHandler<EMailSentEvent>,
        IProcessMessageHandler<CompleteOrderProcessCommand>
    {
        private readonly List<Events.ItemAdded> _items = new List<Events.ItemAdded>();
        private Events.CustomerAssigned _customerAssigned;
        private Events.Initialized _initialized;
        private Events.InvoiceIdAssigned _invoiceIdAssigned;
        private Events.OrderIdAssigned _orderIdAssigned;
        private Events.StatusChanged _statusChanged;
        private Events.TargetSystemAssigned _targetSystemAssigned;

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
            get { return Status.Equals("Colling Off", StringComparison.InvariantCultureIgnoreCase); }
        }

        public Guid CorrelationId { get; private set; }

        public void ProcessMessage(HandlerContext<AcceptOrderProcessCommand> context, EventStream stream)
        {
            stream.AddEvent(ChangeStatus("Order Accepted"));

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

        public Events.Initialized Initialize()
        {
            var now = DateTime.Now;

            return On(new Events.Initialized
            {
                OrderNumber = GenerateOrderNumber(now),
                DateRegistered = now
            });
        }

        public Events.Initialized On(Events.Initialized initialized)
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

        public Events.CustomerAssigned AssignCustomer(string customerName, string customerEMail)
        {
            return On(new Events.CustomerAssigned
            {
                CustomerName = customerName,
                CustomerEMail = customerEMail
            });
        }

        public Events.CustomerAssigned On(Events.CustomerAssigned customerAssigned)
        {
            Guard.AgainstNull(customerAssigned, "customerAssigned");

            return _customerAssigned = customerAssigned;
        }

        public Events.TargetSystemAssigned AssignTargetSystem(string targetSystem, string targetSystemUri)
        {
            return On(new Events.TargetSystemAssigned
            {
                TargetSystem = targetSystem,
                TargetSystemUri = targetSystemUri
            });
        }

        public Events.TargetSystemAssigned On(Events.TargetSystemAssigned targetSystemAssigned)
        {
            Guard.AgainstNull(targetSystemAssigned, "targetSystemAssigned");

            return _targetSystemAssigned = targetSystemAssigned;
        }

        public Events.StatusChanged ChangeStatus(string status)
        {
            return On(new Events.StatusChanged
            {
                Status = status,
                StatusDate = DateTime.Now
            });
        }

        public Events.StatusChanged On(Events.StatusChanged statusChanged)
        {
            Guard.AgainstNull(statusChanged, "statusChanged");

            return _statusChanged = statusChanged;
        }

        public Events.ItemAdded AddItem(Guid productId, string description, decimal price)
        {
            return On(new Events.ItemAdded
            {
                ProductId = productId,
                Description = description,
                Price = price
            });
        }

        public Events.ItemAdded On(Events.ItemAdded itemAdded)
        {
            Guard.AgainstNull(itemAdded, "itemAdded");

            _items.Add(itemAdded);

            Total += itemAdded.Price;

            return itemAdded;
        }

        public Events.InvoiceIdAssigned AssignInvoiceId(Guid invoiceId)
        {
            return On(new Events.InvoiceIdAssigned
            {
                InvoiceId = invoiceId
            });
        }

        public Events.InvoiceIdAssigned On(Events.InvoiceIdAssigned invoiceIdAssigned)
        {
            Guard.AgainstNull(invoiceIdAssigned, "invoiceIdAssigned");

            return _invoiceIdAssigned = invoiceIdAssigned;
        }

        public Events.OrderIdAssigned AssignOrderId(Guid orderId)
        {
            return On(new Events.OrderIdAssigned
            {
                OrderId = orderId
            });
        }

        public Events.OrderIdAssigned On(Events.OrderIdAssigned orderIdAssigned)
        {
            Guard.AgainstNull(orderIdAssigned, "orderIdAssigned");

            return _orderIdAssigned = orderIdAssigned;
        }

        public class Events
        {
            public class Initialized
            {
                public string OrderNumber { get; set; }
                public DateTime DateRegistered { get; set; }
            }

            public class CustomerAssigned
            {
                public string CustomerName { get; set; }
                public string CustomerEMail { get; set; }
            }

            public class TargetSystemAssigned
            {
                public string TargetSystem { get; set; }
                public string TargetSystemUri { get; set; }
            }

            public class StatusChanged
            {
                public string Status { get; set; }
                public DateTime StatusDate { get; set; }
            }

            public class ItemAdded
            {
                public Guid ProductId { get; set; }
                public string Description { get; set; }
                public decimal Price { get; set; }
            }

            public class InvoiceIdAssigned
            {
                public Guid InvoiceId { get; set; }
            }

            public class OrderIdAssigned
            {
                public Guid OrderId { get; set; }
            }
        }

        public void ProcessMessage(HandlerContext<OrderCreatedEvent> context, EventStream stream)
        {
            if (!ShouldProcess(context.TransportMessage, stream))
            {
                return;
            }

            stream.AddEvent(ChangeStatus("Order Created"));
            stream.AddEvent(AssignOrderId(context.Message.OrderId));

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

        public void ProcessMessage(HandlerContext<InvoiceCreatedEvent> context, EventStream stream)
        {
            if (!ShouldProcess(context.TransportMessage, stream))
            {
                return;
            }

            stream.AddEvent(ChangeStatus("Invoice Created"));
            stream.AddEvent(AssignInvoiceId(context.Message.InvoiceId));

            context.Send(new SendEMailCommand
            {
                RecipientEMail = _customerAssigned.CustomerEMail,
                HtmlBody =
                    string.Format(
                        "Hello {0},<br/><br/>Your order number {1} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team",
                        _customerAssigned.CustomerName, _initialized.OrderNumber)
            });
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

        public void ProcessMessage(HandlerContext<EMailSentEvent> context, EventStream stream)
        {
            if (!ShouldProcess(context.TransportMessage, stream))
            {
                return;
            }

            stream.AddEvent(ChangeStatus("Dispatched-EMail Sent"));

            context.Send(new CompleteOrderProcessCommand
            {
                OrderProcessId = CorrelationId
            }, c => c.Local());
        }

        public void ProcessMessage(HandlerContext<CompleteOrderProcessCommand> context, EventStream stream)
        {
            stream.AddEvent(ChangeStatus("Completed"));

            context.Publish(new OrderProcessCompletedEvent
            {
                OrderProcessId = CorrelationId
            });
        }
    }
}