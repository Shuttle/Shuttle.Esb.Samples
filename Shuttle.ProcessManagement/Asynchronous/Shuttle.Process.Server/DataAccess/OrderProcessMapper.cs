using System.Data;
using Shuttle.Core.Data;
using Shuttle.Process.Custom.Server.Domain;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessMapper : IDataRowMapper<OrderProcess>
    {
        public MappedRow<OrderProcess> Map(DataRow row)
        {
            var result = new OrderProcess(OrderProcessColumns.Id.Value(row))
            {
                CustomerName = OrderProcessColumns.CustomerName.Value(row),
                CustomerEMail = OrderProcessColumns.CustomerEMail.Value(row),
                OrderId = OrderProcessColumns.OrderId.Value(row),
                InvoiceId = OrderProcessColumns.InvoiceId.Value(row),
                DateRegistered = OrderProcessColumns.DateRegistered.Value(row),
                OrderNumber = OrderProcessColumns.OrderNumber.Value(row),
                TargetSystem = OrderProcessColumns.TargetSystem.Value(row),
                TargetSystemUri = OrderProcessColumns.TargetSystemUri.Value(row)
            };

            return new MappedRow<OrderProcess>(row, result);
        }
    }
}