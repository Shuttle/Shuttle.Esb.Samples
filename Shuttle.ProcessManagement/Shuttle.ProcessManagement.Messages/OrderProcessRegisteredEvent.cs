using System;
using System.Collections.Generic;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessRegisteredEvent
    {
        public Guid OrderProcessId { get; set; }
        public List<QuotedProduct> QuotedProducts { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}