using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.Custom.Server.Domain
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
            Guard.AgainstNull(id, nameof(id));

            Id = id;
            DateRegistered = DateTime.Now;
        }

        public Guid Id { get; }

        public Guid? OrderId { get; set; }
        public Guid? InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }

        public IEnumerable<OrderProcessItem> OrderProcessItems =>
            new ReadOnlyCollection<OrderProcessItem>(_orderProcessItems);

        public IEnumerable<OrderProcessStatus> Statuses => new ReadOnlyCollection<OrderProcessStatus>(_statuses);

        public DateTime DateRegistered { get; set; }
        public string OrderNumber { get; set; }
        public string TargetSystem { get; set; }
        public string TargetSystemUri { get; set; }

        public void AddItem(OrderProcessItem item)
        {
            Guard.AgainstNull(item, nameof(item));

            _orderProcessItems.Add(item);
        }

        public void AddStatus(OrderProcessStatus status)
        {
            Guard.AgainstNull(status, nameof(status));

            _statuses.Add(status);
        }

        public OrderProcessStatus Status()
        {
            _statuses.Sort((s1, s2) => s2.StatusDate.CompareTo(s1.StatusDate));

            return _statuses.FirstOrDefault();
        }

        public bool CanCancel()
        {
            return Status().Status.Equals("Cooling Off", StringComparison.InvariantCultureIgnoreCase);
        }

        public void GenerateOrderNumber()
        {
            OrderNumber = string.Format("ORD-{0}-{1}", DateRegistered.Ticks.ToString().Substring(8, 6),
                Guid.NewGuid().ToString("N").Substring(6));
        }

        public decimal Total()
        {
            return _orderProcessItems.Aggregate(0m, (current, orderProcessItem) => current + orderProcessItem.Price);
        }

        public bool CanArchive()
        {
            return Status().Status.Equals("Completed");
        }
    }
}