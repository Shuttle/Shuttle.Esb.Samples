using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement
{
    public class OrderProcess
    {
        private readonly List<OrderProcessItem> _orderProcessItems = new List<OrderProcessItem>();
        private readonly List<OrderProcessStatus> _statuses = new List<OrderProcessStatus>();

        public OrderProcess()
            : this(Guid.NewGuid())
        {
        }

        public OrderProcess(Guid id)
        {
            Guard.AgainstNull(id, "id");

            Id = id;
        }

        public Guid Id { get; private set; }

        public Guid? OrderId { get; set; }
        public Guid? InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }

        public IEnumerable<OrderProcessItem> OrderProcessItems
        {
            get { return new ReadOnlyCollection<OrderProcessItem>(_orderProcessItems); }
        }

        public IEnumerable<OrderProcessStatus> Statuses
        {
            get { return new ReadOnlyCollection<OrderProcessStatus>(_statuses); }
        }

        public void AddItem(OrderProcessItem item)
        {
            Guard.AgainstNull(item, "item");

            _orderProcessItems.Add(item);
        }

        public void AddStatus(OrderProcessStatus status)
        {
            Guard.AgainstNull(status, "status");

            _statuses.Add(status);
        }

        public OrderProcessStatus Status()
        {
            _statuses.Sort((s1, s2) => s1.StatusDate.CompareTo(s2.StatusDate));

            return _statuses.FirstOrDefault();
        }
    }
}