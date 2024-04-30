using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Process.Custom.Server.Domain;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessRepository : IOrderProcessRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IDataRowMapper<OrderProcessItem> _orderProcessItemMapper;
        private readonly IDataRowMapper<OrderProcess> _orderProcessMapper;
        private readonly IDataRowMapper<OrderProcessStatus> _orderProcessStatusMapper;
        private readonly IOrderProcessQueryFactory _queryFactory;

        public OrderProcessRepository(IDatabaseGateway databaseGateway, IOrderProcessQueryFactory queryFactory, IDataRowMapper<OrderProcess> orderProcessMapper, IDataRowMapper<OrderProcessItem> orderProcessItemMapper, IDataRowMapper<OrderProcessStatus> orderProcessStatusMapper)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));
            Guard.AgainstNull(orderProcessMapper, nameof(orderProcessMapper));
            Guard.AgainstNull(orderProcessItemMapper, nameof(orderProcessItemMapper));
            Guard.AgainstNull(orderProcessStatusMapper, nameof(orderProcessStatusMapper));

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
            _orderProcessMapper = orderProcessMapper;
            _orderProcessItemMapper = orderProcessItemMapper;
            _orderProcessStatusMapper = orderProcessStatusMapper;
        }

        public async Task AddAsync(OrderProcess orderProcess)
        {
            Guard.AgainstNull(orderProcess, nameof(orderProcess));

            await _databaseGateway.ExecuteAsync(_queryFactory.Add(orderProcess));

            foreach (var orderProcessItem in orderProcess.OrderProcessItems)
            {
                await _databaseGateway.ExecuteAsync(_queryFactory.AddItem(orderProcessItem, orderProcess.Id));
            }

            foreach (var status in orderProcess.Statuses)
            {
                await _databaseGateway.ExecuteAsync(_queryFactory.AddStatus(status, orderProcess.Id));
            }
        }

        public async Task<OrderProcess> GetAsync(Guid id)
        {
            var orderProcessRow = await _databaseGateway.GetRowAsync(_queryFactory.Get(id));

            if (orderProcessRow == null)
            {
                return null;
            }

            var orderProcess = _orderProcessMapper.Map(orderProcessRow).Result;

            foreach (var row in await _databaseGateway.GetRowsAsync(_queryFactory.GetItems(id)))
            {
                orderProcess.AddItem(_orderProcessItemMapper.Map(row).Result);
            }

            foreach (var row in await _databaseGateway.GetRowsAsync(_queryFactory.GetStatuses(id)))
            {
                orderProcess.AddStatus(_orderProcessStatusMapper.Map(row).Result);
            }

            return orderProcess;
        }

        public async Task RemoveAsync(OrderProcess orderProcess)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.Remove(orderProcess.Id));
        }

        public async Task AddStatusAsync(OrderProcessStatus orderProcessStatus, Guid id)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.AddStatus(orderProcessStatus, id));
        }

        public async Task SaveOrderIdAsync(Guid orderId, Guid id)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.SaveOrderId(orderId, id));
        }

        public async Task SaveInvoiceIdAsync(Guid invoiceId, Guid id)
        {
            await _databaseGateway.ExecuteAsync(_queryFactory.SaveInvoiceId(invoiceId, id));
        }
    }
}