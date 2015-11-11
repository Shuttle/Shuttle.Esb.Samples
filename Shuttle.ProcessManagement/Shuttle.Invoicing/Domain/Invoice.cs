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

        public Guid Id { get; private set; }
        public string InvoiceNumber { get; private set; }
        public InvoiceAccountContact AccountContact { get; set; }

        public IEnumerable<InvoiceItem> Items
        {
            get { return new ReadOnlyCollection<InvoiceItem>(_items); }
        }

        public void GenerateInvoiceNumber()
        {
            InvoiceNumber = string.Format("INV-{0}-{1}", InvoiceDate.Ticks.ToString().Substring(8, 6), Guid.NewGuid().ToString("N").Substring(6));
        }

        public DateTime InvoiceDate { get; private set; }
        public Guid OrderId { get; private set; }

        public void AddItem(string description, decimal price)
        {
            _items.Add(new InvoiceItem(description, price));
        }
    }
}