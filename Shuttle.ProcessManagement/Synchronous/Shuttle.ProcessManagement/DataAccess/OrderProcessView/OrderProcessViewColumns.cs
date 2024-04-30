using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement.DataAccess.OrderProcessView
{
    public class OrderProcessViewColumns
    {
        public static readonly Column<Guid> Id = new Column<Guid>("Id", DbType.Guid);
        public static readonly Column<string> CustomerName = new Column<string>("CustomerName", DbType.String, 65);
        public static readonly Column<string> OrderNumber = new Column<string>("OrderNumber", DbType.String, 20);
        public static readonly Column<DateTime?> OrderDate = new Column<DateTime?>("OrderDate", DbType.DateTime);
        public static readonly Column<decimal?> OrderTotal = new Column<decimal?>("OrderTotal", DbType.Decimal);
        public static readonly Column<string> Status = new Column<string>("Status", DbType.String, 35);
        public static readonly Column<string> TargetSystem = new Column<string>("TargetSystem", DbType.String, 65);
        public static readonly Column<string> TargetSystemUri = new Column<string>("TargetSystemUri", DbType.String, 130);
    }
}