using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessViewColumns
    {
        public static readonly MappedColumn<Guid> Id = new MappedColumn<Guid>("Id", DbType.Guid);
        public static readonly MappedColumn<string> CustomerName = new MappedColumn<string>("CustomerName", DbType.String, 65);
        public static readonly MappedColumn<string> OrderNumber = new MappedColumn<string>("OrderNumber", DbType.String, 20);
        public static readonly MappedColumn<DateTime?> OrderDate = new MappedColumn<DateTime?>("OrderDate", DbType.DateTime);
        public static readonly MappedColumn<decimal?> OrderTotal = new MappedColumn<decimal?>("OrderTotal", DbType.Decimal);
        public static readonly MappedColumn<string> Status = new MappedColumn<string>("Status", DbType.String, 35);
    }
}