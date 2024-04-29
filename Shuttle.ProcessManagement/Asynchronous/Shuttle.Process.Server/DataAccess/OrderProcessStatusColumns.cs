using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement;

public class OrderProcessStatusColumns
{
    public static readonly Column<Guid> OrderProcessId = new("OrderProcessId", DbType.Guid);
    public static readonly Column<string> Status = new("Status", DbType.String, 35);
    public static readonly Column<DateTime> StatusDate = new("StatusDate", DbType.DateTime);
}