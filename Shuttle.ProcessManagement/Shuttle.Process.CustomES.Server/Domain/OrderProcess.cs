using System;
using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.EMailSender.Messages;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;

namespace Shuttle.Process.CustomES.Server.Domain
{
    public class OrderProcess
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

        public OrderProcess(Guid id)
        {
            Id = id;
        }

        public decimal Total { get; private set; }

        public Guid Id { get; }

        public string Status => _statusChanged != null ? _statusChanged.Status : string.Empty;

        public bool CanArchive => Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase);

        public bool CanCancel => Status.Equals("Cooling Off", StringComparison.InvariantCultureIgnoreCase);

        public Events.Initialized Initialize()
        {
            var now = DateTime.Now;

            return On(new Events.Initialized
            {
                OrderNumber = GenerateOrderNumber(now),
                DateRegistered = now
            });
        }

        private Events.Initialized On(Events.Initialized initialized)
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

        private Events.CustomerAssigned On(Events.CustomerAssigned customerAssigned)
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

        private Events.TargetSystemAssigned On(Events.TargetSystemAssigned targetSystemAssigned)
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

        private Events.StatusChanged On(Events.StatusChanged statusChanged)
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

        private Events.ItemAdded On(Events.ItemAdded itemAdded)
        {
            Guard.AgainstNull(itemAdded, "itemAdded");

            _items.Add(itemAdded);

            Total += itemAdded.Price;

            return itemAdded;
        }

        public CreateOrderCommand CreateOrderCommand()
        {
            var result = new CreateOrderCommand
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

        public Events.InvoiceIdAssigned AssignInvoiceId(Guid invoiceId)
        {
            return On(new Events.InvoiceIdAssigned
            {
                InvoiceId = invoiceId
            });
        }

        private Events.InvoiceIdAssigned On(Events.InvoiceIdAssigned invoiceIdAssigned)
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

        private Events.OrderIdAssigned On(Events.OrderIdAssigned orderIdAssigned)
        {
            Guard.AgainstNull(orderIdAssigned, "orderIdAssigned");

            return _orderIdAssigned = orderIdAssigned;
        }

        public SendEMailCommand SendEMailCommand()
        {
            return new SendEMailCommand
            {
                RecipientEMail = _customerAssigned.CustomerEMail,
                HtmlBody =
                    string.Format(
                        "Hello {0},<br/><br/>Your order number {1} has been dispatched.<br/><br/>Regards,<br/>The Shuttle Books Team",
                        _customerAssigned.CustomerName, _initialized.OrderNumber)
            };
        }

        public CreateInvoiceCommand CreateInvoiceCommand()
        {
            var result = new CreateInvoiceCommand
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
    }
}