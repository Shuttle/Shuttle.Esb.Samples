using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceColumns
    {
        public static readonly Column<Guid> Id = new Column<Guid>("Id", DbType.Guid);
        public static readonly Column<string> InvoiceNumber = new Column<string>("InvoiceNumber", DbType.String, 20);
        public static readonly Column<string> InvoiceDate = new Column<string>("InvoiceDate", DbType.DateTime);
        public static readonly Column<Guid> OrderId = new Column<Guid>("OrderId", DbType.Guid);
        public static readonly Column<string> AccountContactName = new Column<string>("AccountContactName", DbType.String, 65);
        public static readonly Column<string> AccountContactEMail = new Column<string>("AccountContactEMail", DbType.String, 130);
    }
}