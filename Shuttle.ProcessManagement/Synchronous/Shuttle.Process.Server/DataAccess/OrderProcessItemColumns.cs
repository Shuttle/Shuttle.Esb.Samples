using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement;

public class OrderProcessItemColumns
{
    public static readonly Column<Guid> OrderProcessId = new("OrderProcessId", DbType.Guid);
    public static readonly Column<Guid> ProductId = new("ProductId", DbType.Guid);
    public static readonly Column<string> Description = new("Description", DbType.String, 130);
    public static readonly Column<decimal> Price = new("Price", DbType.Decimal);
}