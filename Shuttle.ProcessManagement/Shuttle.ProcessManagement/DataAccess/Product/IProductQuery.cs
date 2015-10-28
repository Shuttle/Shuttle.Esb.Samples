using System.Collections.Generic;
using System.Data;

namespace Shuttle.ProcessManagement
{
    public interface IProductQuery
    {
        IEnumerable<DataRow> All();
    }
}