using System;
using System.Collections.Generic;

namespace Shuttle.ProcessManagement.Messages
{
    public class OrderProcessRegistered
    {
        public Guid OrderProcessId { get; set; }
        public List<QuotedProduct> QuotedProducts { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string TargetSystem { get; set; }
        public string TargetSystemUri { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
    }
}