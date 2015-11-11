using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Process.Custom.Server.Domain;

namespace Shuttle.ProcessManagement
{
    public class OrderProcessRepository : IOrderProcessRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderProcessQueryFactory _queryFactory;
        private readonly IDataRowMapper<OrderProcess> _orderProcessMapper;
        private readonly IDataRowMapper<OrderProcessItem> _orderProcessItemMapper;
        private readonly IDataRowMapper<OrderProcessStatus> _orderProcessStatusMapper;

        public OrderProcessRepository(IDatabaseGateway databaseGateway, IOrderProcessQueryFactory queryFactory, IDataRowMapper<OrderProcess> orderProcessMapper, IDataRowMapper<OrderProcessItem> orderProcessItemMapper, IDataRowMapper<OrderProcessStatus> orderProcessStatusMapper)
        {
            Guard.AgainstNull(databaseGateway, "databaseGateway");
            Guard.AgainstNull(queryFactory, "queryFactory");
            Guard.AgainstNull(orderProcessMapper, "orderProcessMapper");
            Guard.AgainstNull(orderProcessItemMapper, "orderProcessItemMapper");
            Guard.AgainstNull(orderProcessStatusMapper, "orderProcessStatusMapper");

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
            _orderProcessMapper = orderProcessMapper;
            _orderProcessItemMapper = orderProcessItemMapper;
            _orderProcessStatusMapper = orderProcessStatusMapper;
        }

        public void Add(OrderProcess orderProcess)
        {
            Guard.AgainstNull(orderProcess, "orderProcess");

            _databaseGateway.ExecuteUsing(_queryFactory.Add(orderProcess));

            foreach (var orderProcessItem in orderProcess.OrderProcessItems)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.AddItem(orderProcessItem, orderProcess.Id));
            }

            foreach (var status in orderProcess.Statuses)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.AddStatus(status, orderProcess.Id));
            }
        }

        public OrderProcess Get(Guid id)
        {
            var orderProcessRow = _databaseGateway.GetSingleRowUsing(_queryFactory.Get(id));

            if (orderProcessRow == null)
            {
                return null;
            }

            var orderProcess = _orderProcessMapper.Map(orderProcessRow).Result;

            foreach (var row in _databaseGateway.GetRowsUsing(_queryFactory.GetItems(id)))
            {
                orderProcess.AddItem(_orderProcessItemMapper.Map(row).Result);
            }

            foreach (var row in _databaseGateway.GetRowsUsing(_queryFactory.GetStatuses(id)))
            {
                orderProcess.AddStatus(_orderProcessStatusMapper.Map(row).Result);
            }

            return orderProcess;
        }

        public void Remove(OrderProcess orderProcess)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Remove(orderProcess.Id));
        }

        public void AddStatus(OrderProcessStatus orderProcessStatus, Guid id)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.AddStatus(orderProcessStatus, id));
        }

        public void SaveOrderId(Guid orderId, Guid id)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.SaveOrderId(orderId, id));
        }

        public void SaveInvoiceId(Guid invoiceId, Guid id)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.SaveInvoiceId(invoiceId, id));
        }
    }
}