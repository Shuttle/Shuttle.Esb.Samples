using System;
using System.Collections.Generic;
using System.Data;

namespace Shuttle.ProcessManagement.DataAccess.Product
{
    public interface IProductQuery
    {
        IEnumerable<DataRow> All();
        DataRow Get(Guid id);
    }
}