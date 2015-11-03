using System.Collections.Generic;

namespace Shuttle.ProcessManagement.Messages
{
    public class RegisterOrderProcessCommand
    {
        public RegisterOrderProcessCommand()
        {
            QuotedProducts = new List<QuotedProduct>();
        }

        public List<QuotedProduct> QuotedProducts { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEMail { get; set; }
        public string TargetSystem { get; set; }
        public string TargetSystemUri { get; set; }
    }
}