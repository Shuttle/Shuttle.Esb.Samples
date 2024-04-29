using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Ordering.DataAccess
{
    public class OrderColumns
    {
        public static readonly Column<Guid> Id = new Column<Guid>("Id", DbType.Guid);
        public static readonly Column<string> OrderNumber = new Column<string>("OrderNumber", DbType.String, 20);
        public static readonly Column<string> OrderDate = new Column<string>("OrderDate", DbType.DateTime);
        public static readonly Column<string> CustomerName = new Column<string>("CustomerName", DbType.String, 65);
        public static readonly Column<string> CustomerEMail = new Column<string>("CustomerEMail", DbType.String, 130);
    }
}