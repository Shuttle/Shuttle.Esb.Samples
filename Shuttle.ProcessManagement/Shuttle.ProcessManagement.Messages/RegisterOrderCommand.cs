using System.Collections.Generic;

namespace Shuttle.ProcessManagement.Messages
{
    public class RegisterOrderCommand
    {
        public List<QuotedProduct> QuotedProducts { get; set; }
    }
}