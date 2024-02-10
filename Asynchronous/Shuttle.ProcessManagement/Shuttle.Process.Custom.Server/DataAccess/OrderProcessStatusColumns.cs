using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessStatusColumns
    {
        public static readonly MappedColumn<Guid> OrderProcessId =
            new MappedColumn<Guid>("OrderProcessId", DbType.Guid);

        public static readonly MappedColumn<string> Status =
            new MappedColumn<string>("Status", DbType.String, 35);

        public static readonly MappedColumn<DateTime> StatusDate =
            new MappedColumn<DateTime>("StatusDate", DbType.DateTime);
    }
}