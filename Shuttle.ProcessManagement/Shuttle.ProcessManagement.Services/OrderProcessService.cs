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
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;
        private readonly IOrderProcessRepository _orderProcessRepository;

        public OrderProcessService(IServiceBus bus, IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery, IOrderProcessRepository orderProcessRepository)
        {
            Guard.AgainstNull(bus, "bus");
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");
            Guard.AgainstNull(orderProcessRepository, "orderProcessRepository");

            _bus = bus;
            _databaseContextFactory = databaseContextFactory;
            _orderProcessViewQuery = orderProcessViewQuery;
            _orderProcessRepository = orderProcessRepository;
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
                           Status = OrderProcessViewColumns.Status.MapFrom(row),
                           CanCancel = status == "cooling off",
                           CanArchive = status == "completed"
                       };
            }
        }

        public void CancelOrder(Guid id)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = _orderProcessRepository.Get(id);

                if (!orderProcess.CanCancel())
                {
                    return;
                }

                _bus.Send(new CancelOrderProcessCommand
                {
                    OrderProcessId = id
                }, c => c.WithRecipient(orderProcess.TargetSystemUri));

                _orderProcessViewQuery.SaveStatus(id, "Cancelling");
            }
        }

        public void ArchiveOrder(Guid id)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var orderProcess = _orderProcessRepository.Get(id);

                if (!orderProcess.CanArchive())
                {
                    return;
                }

                _bus.Send(new ArchiveOrderProcessCommand
                {
                    OrderProcessId = id
                }, c => c.WithRecipient(orderProcess.TargetSystemUri));

                _orderProcessViewQuery.SaveStatus(id, "Archiving");
            }
        }
    }
}