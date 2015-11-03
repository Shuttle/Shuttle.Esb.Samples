using System;
using System.Linq;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.Services
{
    public class OrderProcessService : IOrderProcessService
    {
        private readonly IServiceBus _bus;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrderProcessService(IServiceBus bus, IDatabaseContextFactory databaseContextFactory, IDatabaseGateway databaseGateway, IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(bus, "bus");
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(databaseGateway, "databaseGateway");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _bus = bus;
            _databaseContextFactory = databaseContextFactory;
            _databaseGateway = databaseGateway;
            _orderProcessViewQuery = orderProcessViewQuery;
        }

        public dynamic ActiveOrders()
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                return from row in _orderProcessViewQuery.All()
                       let status = (OrderProcessViewColumns.Status.MapFrom(row) ?? string.Empty).ToLower()
                       select new
                       {
                           Id = OrderProcessViewColumns.Id.MapFrom(row),
                           CustomerName = OrderProcessViewColumns.CustomerName.MapFrom(row),
                           OrderNumber = OrderProcessViewColumns.OrderNumber.MapFrom(row),
                           OrderDate = OrderProcessViewColumns.OrderDate.MapFrom(row),
                           OrderTotal = OrderProcessViewColumns.OrderTotal.MapFrom(row),
                           Status = status,
                           CanCancel = status == "cooling off"
                       };
            }
        }

        public void CancelOrder(Guid id)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var row = _orderProcessViewQuery.Find(id);

                _bus.Send(new CancelOrderProcessCommand
                {
                    Id = id
                }, c => c.WithRecipient(OrderProcessViewColumns.TargetSystemUri.MapFrom(row)));

                _orderProcessViewQuery.Cancelling(id);
            }
        }
    }
}