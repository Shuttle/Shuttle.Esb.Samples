using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessItemMapper : IDataRowMapper<OrderProcessItem>
    {
        public MappedRow<OrderProcessItem> Map(DataRow row)
        {
            return new MappedRow<OrderProcessItem>(row,
                new OrderProcessItem(
                    OrderProcessItemColumns.ProductId.MapFrom(row),
                    OrderProcessItemColumns.Description.MapFrom(row),
                    OrderProcessItemColumns.Price.MapFrom(row)
                ));
        }
    }
}