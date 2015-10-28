using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class ProductColumns
    {
        public static readonly MappedColumn<Guid> Id =
            new MappedColumn<Guid>("Id", DbType.Guid);

        public static readonly MappedColumn<string> Description =
            new MappedColumn<string>("Description", DbType.String, 130);

        public static readonly MappedColumn<decimal> Price =
            new MappedColumn<decimal>("Price", DbType.Decimal);
    }
}