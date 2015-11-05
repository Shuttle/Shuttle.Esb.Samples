using System;
using System.Collections.Generic;

namespace Shuttle.Invoicing.Messages
{
    public class InvoiceCreatedEvent
    {
        public InvoiceCreatedEvent()
        {
            Items = new List<MessageInvoiceItem>();
        }

        public Guid OrderId { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string AccountContactName { get; set; }
        public string AccountContactEMail { get; set; }

        public List<MessageInvoiceItem> Items { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}