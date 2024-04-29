using System;
using System.Threading.Tasks;

namespace Shuttle.ProcessManagement.Domain
{
    public interface IOrderProcessService
    {
        Task<dynamic> ActiveOrdersAsync();
        Task CancelOrderAsync(Guid id);
        Task ArchiveOrderAsync(Guid id);
    }
}