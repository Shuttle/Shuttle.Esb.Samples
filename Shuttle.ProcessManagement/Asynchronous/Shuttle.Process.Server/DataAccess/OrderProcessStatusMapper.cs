using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessStatusMapper : IDataRowMapper<OrderProcessStatus>
    {
        public MappedRow<OrderProcessStatus> Map(DataRow row)
        {
            return new MappedRow<OrderProcessStatus>(row,
                new OrderProcessStatus(
                    OrderProcessStatusColumns.Status.Value(row),
                    OrderProcessStatusColumns.StatusDate.Value(row)
                ));
        }
    }
}