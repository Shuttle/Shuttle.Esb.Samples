using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderItemColumns
    {
        public static readonly MappedColumn<Guid> OrderId =
            new MappedColumn<Guid>("OrderId", DbType.Guid);

        public static readonly MappedColumn<string> Description =
            new MappedColumn<string>("Description", DbType.String, 130);

        public static readonly MappedColumn<decimal> Price =
            new MappedColumn<decimal>("Price", DbType.Decimal);
    }
}