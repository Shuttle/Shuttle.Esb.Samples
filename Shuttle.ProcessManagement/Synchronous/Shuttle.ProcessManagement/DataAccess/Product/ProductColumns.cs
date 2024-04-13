using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement.DataAccess.Product
{
    public class ProductColumns
    {
        public static readonly Column<Guid> Id = new Column<Guid>("Id", DbType.Guid);
        public static readonly Column<string> Description = new Column<string>("Description", DbType.String, 130);
        public static readonly Column<decimal> Price = new Column<decimal>("Price", DbType.Decimal);
        public static readonly Column<string> Url = new Column<string>("Url", DbType.String, 130);
    }
}