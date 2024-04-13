namespace Shuttle.Ordering.Domain
{
    public interface IOrderRepository
    {
        void Add(Order order);
    }
}