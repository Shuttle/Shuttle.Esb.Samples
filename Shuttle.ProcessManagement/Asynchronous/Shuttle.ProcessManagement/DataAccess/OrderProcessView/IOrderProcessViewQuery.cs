using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.DataAccess.OrderProcessView
{
    public interface IOrderProcessViewQuery
    {
        Task<IEnumerable<DataRow>> AllAsync();
        Task AddAsync(OrderProcessRegistered message);
        Task<DataRow> FindAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task SaveStatusAsync(Guid orderProcessId, string status);
    }
}