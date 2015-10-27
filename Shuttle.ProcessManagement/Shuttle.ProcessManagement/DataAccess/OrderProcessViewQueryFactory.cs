using Shuttle.Core.Data;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessViewQueryFactory : IOrderProcessViewQueryFactory
    {
        public IQuery All()
        {
            return new RawQuery(@"select Id, OrderNumber, OrderDate, Status from OrderProcessView order by OrderDate");
        }
    }
}