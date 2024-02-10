using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessViewQuery
    {
        IEnumerable<DataRow> All();
        void Add(OrderProcessRegistered message);
        DataRow Find(Guid id);
        void Remove(Guid id);
        void SaveStatus(Guid orderProcessId, string status);
    }
}