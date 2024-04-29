using System.Threading.Tasks;

namespace Shuttle.Ordering.Domain
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
    }
}