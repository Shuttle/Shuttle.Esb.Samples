using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shuttle.Ordering.Domain
{
    public class Order
    {
        private readonly List<OrderItem> _items = new List<OrderItem>();

        public Order(string orderNumber, DateTime orderDate)
            : this(Guid.NewGuid(), orderNumber, orderDate)
        {
        }

        public Order(Guid id, string orderNumber, DateTime orderDate)
        {
            Id = id;
            OrderNumber = orderNumber;
            OrderDate = orderDate;
        }

        public Guid Id { get; }
        public string OrderNumber { get; }
        public OrderCustomer Customer { get; set; }

        public IEnumerable<OrderItem> Items => new ReadOnlyCollection<OrderItem>(_items);

        public DateTime OrderDate { get; }

        public void AddItem(string description, decimal price)
        {
            _items.Add(new OrderItem(description, price));
        }
    }
}