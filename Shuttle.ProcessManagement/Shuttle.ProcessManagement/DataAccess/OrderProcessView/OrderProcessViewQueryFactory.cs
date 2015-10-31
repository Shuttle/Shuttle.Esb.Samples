using Shuttle.Core.Data;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessViewQueryFactory : IOrderProcessViewQueryFactory
    {
        public IQuery All()
        {
            return RawQuery.Create(@"select Id, CustomerName, OrderNumber, OrderDate, Status from OrderProcessView order by OrderDate");
        }

        public IQuery Add(OrderProcessRegisteredEvent message)
        {
            return RawQuery.Create(@"
insert into dbo.OrderProcessView
(
    Id,
    CustomerName,
    Status
)
values
(
    @Id,
    @CustomerName,
    @Status
)
")
                .AddParameterValue(OrderProcessViewColumns.Id, message.OrderProcessId)
                .AddParameterValue(OrderProcessViewColumns.CustomerName, message.CustomerName)
                .AddParameterValue(OrderProcessViewColumns.Status, message.Status);
        }
    }
}