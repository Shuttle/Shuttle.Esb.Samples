using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessItemColumns
    {
        public static readonly MappedColumn<Guid> OrderProcessId =
            new MappedColumn<Guid>("OrderProcessId", DbType.Guid);

        public static readonly MappedColumn<Guid> ProductId =
            new MappedColumn<Guid>("ProductId", DbType.Guid);

        public static readonly MappedColumn<string> Description =
            new MappedColumn<string>("Description", DbType.String, 130);

        public static readonly MappedColumn<decimal> Price =
            new MappedColumn<decimal>("Price", DbType.Decimal);
    }
}