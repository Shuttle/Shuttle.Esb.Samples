using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceItemColumns
    {
        public static readonly Column<Guid> InvoiceId = new Column<Guid>("InvoiceId", DbType.Guid);
        public static readonly Column<string> Description = new Column<string>("Description", DbType.String, 130);
        public static readonly Column<decimal> Price = new Column<decimal>("Price", DbType.Decimal);
    }
}