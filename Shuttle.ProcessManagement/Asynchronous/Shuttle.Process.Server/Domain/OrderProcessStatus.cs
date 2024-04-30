using System;
using Shuttle.Core.Contract;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessStatus
    {
        public OrderProcessStatus(string status)
            : this(status, DateTime.Now)
        {
        }

        public OrderProcessStatus(string status, DateTime statusDate)
        {
            Guard.AgainstNullOrEmptyString(status, "status");

            Status = status;
            StatusDate = statusDate;
        }

        public string Status { get; }
        public DateTime StatusDate { get; }
    }
}