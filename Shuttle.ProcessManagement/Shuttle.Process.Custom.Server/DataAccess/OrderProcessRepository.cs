using System;
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

        public OrderProcessRepository(IDatabaseGateway databaseGateway, IOrderProcessQueryFactory queryFactory,
            IDataRowMapper<OrderProcess> orderProcessMapper, IDataRowMapper<OrderProcessItem> orderProcessItemMapper,
            IDataRowMapper<OrderProcessStatus> orderProcessStatusMapper)
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

        public void Add(OrderProcess orderProcess)
        {
            Guard.AgainstNull(orderProcess, nameof(orderProcess));

            _databaseGateway.Execute(_queryFactory.Add(orderProcess));

            foreach (var orderProcessItem in orderProcess.OrderProcessItems)
            {
                _databaseGateway.Execute(_queryFactory.AddItem(orderProcessItem, orderProcess.Id));
            }

            foreach (var status in orderProcess.Statuses)
            {
                _databaseGateway.Execute(_queryFactory.AddStatus(status, orderProcess.Id));
            }
        }

        public OrderProcess Get(Guid id)
        {
            var orderProcessRow = _databaseGateway.GetRow(_queryFactory.Get(id));

            if (orderProcessRow == null)
            {
                return null;
            }

            var orderProcess = _orderProcessMapper.Map(orderProcessRow).Result;

            foreach (var row in _databaseGateway.GetRows(_queryFactory.GetItems(id)))
            {
                orderProcess.AddItem(_orderProcessItemMapper.Map(row).Result);
            }

            foreach (var row in _databaseGateway.GetRows(_queryFactory.GetStatuses(id)))
            {
                orderProcess.AddStatus(_orderProcessStatusMapper.Map(row).Result);
            }

            return orderProcess;
        }

        public void Remove(OrderProcess orderProcess)
        {
            _databaseGateway.Execute(_queryFactory.Remove(orderProcess.Id));
        }

        public void AddStatus(OrderProcessStatus orderProcessStatus, Guid id)
        {
            _databaseGateway.Execute(_queryFactory.AddStatus(orderProcessStatus, id));
        }

        public void SaveOrderId(Guid orderId, Guid id)
        {
            _databaseGateway.Execute(_queryFactory.SaveOrderId(orderId, id));
        }

        public void SaveInvoiceId(Guid invoiceId, Guid id)
        {
            _databaseGateway.Execute(_queryFactory.SaveInvoiceId(invoiceId, id));
        }
    }
}