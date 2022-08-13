using System;
using System.Collections.Generic;

namespace Shuttle.Ordering.Messages
{
    public class OrderCreated
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }

        public List<MessageOrderItem> Items { get; set; } = new List<MessageOrderItem>();
    }
}