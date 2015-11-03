using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessViewQuery
    {
        IEnumerable<DataRow> All();
        void Add(OrderProcessRegisteredEvent message);
        DataRow Find(Guid id);
        void Cancelling(Guid id);
        void Remove(Guid id);
    }
}