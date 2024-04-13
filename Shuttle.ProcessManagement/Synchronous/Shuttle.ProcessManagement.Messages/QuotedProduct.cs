using System;

namespace Shuttle.ProcessManagement.Messages
{
    public class QuotedProduct
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}