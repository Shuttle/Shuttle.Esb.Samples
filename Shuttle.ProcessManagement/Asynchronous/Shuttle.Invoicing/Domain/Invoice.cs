using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shuttle.Invoicing.Domain
{
    public class Invoice
    {
        private readonly List<InvoiceItem> _items = new List<InvoiceItem>();

        public Invoice(Guid orderId)
            : this(Guid.NewGuid(), orderId, DateTime.Now)
        {
        }

        public Invoice(Guid id, Guid orderId, DateTime invoiceDate)
        {
            Id = id;
            OrderId = orderId;
            InvoiceDate = invoiceDate;
        }

        public Guid Id { get; }
        public string InvoiceNumber { get; private set; }
        public InvoiceAccountContact AccountContact { get; set; }

        public IEnumerable<InvoiceItem> Items => new ReadOnlyCollection<InvoiceItem>(_items);

        public DateTime InvoiceDate { get; }
        public Guid OrderId { get; }

        public void GenerateInvoiceNumber()
        {
            InvoiceNumber =
                $"INV-{InvoiceDate.Ticks.ToString().Substring(8, 6)}-{Guid.NewGuid().ToString("N")[6..]}";
        }

        public void AddItem(string description, decimal price)
        {
            _items.Add(new InvoiceItem(description, price));
        }
    }
}