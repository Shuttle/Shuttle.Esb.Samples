using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceColumns
    {
        public static readonly MappedColumn<Guid> Id =
            new MappedColumn<Guid>("Id", DbType.Guid);

        public static readonly MappedColumn<string> InvoiceNumber =
            new MappedColumn<string>("InvoiceNumber", DbType.String, 20);

        public static readonly MappedColumn<string> InvoiceDate =
            new MappedColumn<string>("InvoiceDate", DbType.DateTime);

        public static readonly MappedColumn<Guid> OrderId =
            new MappedColumn<Guid>("OrderId", DbType.Guid);

        public static readonly MappedColumn<string> AccountContactName =
            new MappedColumn<string>("AccountContactName", DbType.String, 65);

        public static readonly MappedColumn<string> AccountContactEMail =
            new MappedColumn<string>("AccountContactEMail", DbType.String, 130);
    }
}