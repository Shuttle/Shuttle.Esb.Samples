using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.HandRolled
{
    public class OrderProcessStatus
    {
        public OrderProcessStatus(string status, DateTime statusDate)
        {
            Guard.AgainstNullOrEmptyString(status, "status");

            Status = status;
            StatusDate = statusDate;
        }

        public string Status { get; private set; }
        public DateTime StatusDate { get; private set; }
    }
}