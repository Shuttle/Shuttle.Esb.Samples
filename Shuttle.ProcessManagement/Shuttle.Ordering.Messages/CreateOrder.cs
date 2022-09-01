using System;
using System.Collections.Generic;

namespace Shuttle.Ordering.Messages
{
    public class CreateOrder
    {
        public CreateOrder()
        {
            Items = new List<MessageOrderItem>();
        }

        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }

        public List<MessageOrderItem> Items { get; set; }
    }
}