using System.Collections.Generic;
using System.Data;

namespace Shuttle.ProcessManagement
{
    public interface IOrderProcessViewQuery
    {
        IEnumerable<DataRow> All();
    }
}