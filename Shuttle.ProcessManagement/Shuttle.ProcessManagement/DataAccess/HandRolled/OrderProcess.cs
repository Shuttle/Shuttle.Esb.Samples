using System;
using System.Collections.Generic;

namespace Shuttle.ProcessManagement.HandRolled
{
    public class OrderProcess
    {
        public Guid Id { get; private set; }
        
        private List<OrderProcessItem> _items = new List<OrderProcessItem>();

        private List<OrderProcessStatus> _statusItems = new List<OrderProcessStatus>();
    }
}