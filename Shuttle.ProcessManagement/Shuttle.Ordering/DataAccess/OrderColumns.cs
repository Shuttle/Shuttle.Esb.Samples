using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderColumns
    {
        public static readonly MappedColumn<Guid> Id =
            new MappedColumn<Guid>("Id", DbType.Guid);

        public static readonly MappedColumn<string> OrderNumber =
            new MappedColumn<string>("OrderNumber", DbType.String, 20);

        public static readonly MappedColumn<string> OrderDate =
            new MappedColumn<string>("OrderDate", DbType.DateTime);

        public static readonly MappedColumn<string> CustomerName =
        new MappedColumn<string>("CustomerName", DbType.String, 65);

        public static readonly MappedColumn<string> CustomerEMail =
            new MappedColumn<string>("CustomerEMail", DbType.String, 130);
    }
}