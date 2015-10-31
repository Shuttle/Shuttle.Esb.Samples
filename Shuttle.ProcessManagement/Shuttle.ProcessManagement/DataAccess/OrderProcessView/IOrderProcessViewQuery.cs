using System.Collections.Generic;
using System.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessViewQuery
    {
        IEnumerable<DataRow> All();
        void Add(OrderProcessRegisteredEvent message);
    }
}