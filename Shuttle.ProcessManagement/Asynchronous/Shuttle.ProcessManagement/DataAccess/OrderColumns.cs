using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement.DataAccess
{
    public class OrderColumns
    {
        public static readonly Column<Guid> Id = new Column<Guid>("Id", DbType.Guid);
        public static readonly Column<string> OrderNumber = new Column<string>("OrderNumber", DbType.String, 20);
    }
}