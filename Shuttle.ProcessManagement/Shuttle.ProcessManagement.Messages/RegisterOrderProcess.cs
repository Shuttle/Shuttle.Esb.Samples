using System.Collections.Generic;

namespace Shuttle.ProcessManagement.Messages
{
    public class RegisterOrderProcess
    {
        public List<QuotedProduct> QuotedProducts { get; set; } = new List<QuotedProduct>();
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }
        public string TargetSystem { get; set; }
        public string TargetSystemUri { get; set; }
    }
}