using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement.DataAccess
{
    public class OrderColumns
    {
        public static readonly MappedColumn<Guid> Id = new MappedColumn<Guid>("Id", DbType.Guid);
        public static readonly MappedColumn<string> OrderNumber = new MappedColumn<string>("OrderNumber", DbType.String, 20);
    }
}