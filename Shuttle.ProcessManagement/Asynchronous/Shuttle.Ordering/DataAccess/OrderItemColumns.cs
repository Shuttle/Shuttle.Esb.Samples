using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderItemColumns
    {
        public static readonly Column<Guid> OrderId = new Column<Guid>("OrderId", DbType.Guid);
        public static readonly Column<string> Description = new Column<string>("Description", DbType.String, 130);
        public static readonly Column<decimal> Price = new Column<decimal>("Price", DbType.Decimal);
    }
}