using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement;

public class OrderProcessColumns
{
    public static readonly Column<Guid> Id = new("Id", DbType.Guid);
    public static readonly Column<Guid?> OrderId = new("OrderId", DbType.Guid);
    public static readonly Column<Guid?> InvoiceId = new("InvoiceId", DbType.Guid);
    public static readonly Column<string> CustomerName = new("CustomerName", DbType.String, 65);
    public static readonly Column<string> CustomerEMail = new("CustomerEMail", DbType.String, 130);
    public static readonly Column<DateTime> DateRegistered = new("DateRegistered", DbType.DateTime);
    public static readonly Column<string> OrderNumber = new("OrderNumber", DbType.String, 20);
    public static readonly Column<string> TargetSystem = new("TargetSystem", DbType.String, 65);
    public static readonly Column<string> TargetSystemUri = new("TargetSystemUri", DbType.String, 130);
}