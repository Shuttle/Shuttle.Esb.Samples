using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessColumns
    {
        public static readonly MappedColumn<Guid> Id =
            new MappedColumn<Guid>("Id", DbType.Guid);

        public static readonly MappedColumn<Guid?> OrderId =
            new MappedColumn<Guid?>("OrderId", DbType.Guid);

        public static readonly MappedColumn<Guid?> InvoiceId =
            new MappedColumn<Guid?>("InvoiceId", DbType.Guid);

        public static readonly MappedColumn<string> CustomerName =
            new MappedColumn<string>("CustomerName", DbType.String, 65);

        public static readonly MappedColumn<string> CustomerEMail =
            new MappedColumn<string>("CustomerEMail", DbType.String, 130);

        public static readonly MappedColumn<DateTime> DateRegistered =
            new MappedColumn<DateTime>("DateRegistered", DbType.DateTime);

        public static readonly MappedColumn<string> OrderNumber =
            new MappedColumn<string>("OrderNumber", DbType.String, 20);

        public static readonly MappedColumn<string> TargetSystem =
            new MappedColumn<string>("TargetSystem", DbType.String, 65);

        public static readonly MappedColumn<string> TargetSystemUri =
            new MappedColumn<string>("TargetSystemUri", DbType.String, 130);
    }
}