using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessMapper : IDataRowMapper<OrderProcess>
    {
        public MappedRow<OrderProcess> Map(DataRow row)
        {
            var result = new OrderProcess(OrderProcessColumns.Id.MapFrom(row))
            {
                CustomerName = OrderProcessColumns.CustomerName.MapFrom(row),
                CustomerEMail = OrderProcessColumns.CustomerEMail.MapFrom(row),
                OrderId = OrderProcessColumns.OrderId.MapFrom(row),
                InvoiceId = OrderProcessColumns.InvoiceId.MapFrom(row),
                DateRegistered = OrderProcessColumns.DateRegistered.MapFrom(row),
                OrderNumber = OrderProcessColumns.OrderNumber.MapFrom(row),
                TargetSystem= OrderProcessColumns.TargetSystem.MapFrom(row),
                TargetSystemUri = OrderProcessColumns.TargetSystemUri.MapFrom(row)
            };

            return new MappedRow<OrderProcess>(row, result);
        }
    }
}