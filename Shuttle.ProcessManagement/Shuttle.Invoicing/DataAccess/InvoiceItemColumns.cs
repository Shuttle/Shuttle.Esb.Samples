using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceItemColumns
    {
        public static readonly MappedColumn<Guid> InvoiceId =
            new MappedColumn<Guid>("InvoiceId", DbType.Guid);

        public static readonly MappedColumn<string> Description =
            new MappedColumn<string>("Description", DbType.String, 130);

        public static readonly MappedColumn<decimal> Price =
            new MappedColumn<decimal>("Price", DbType.Decimal);
    }
}