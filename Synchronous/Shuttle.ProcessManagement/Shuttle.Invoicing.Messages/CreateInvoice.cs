using System;
using System.Collections.Generic;

namespace Shuttle.Invoicing.Messages
{
    public class CreateInvoice
    {
        public Guid OrderId { get; set; }
        public string AccountContactName { get; set; }
        public string AccountContactEMail { get; set; }

        public List<MessageInvoiceItem> Items { get; set; } = new List<MessageInvoiceItem>();
    }
}