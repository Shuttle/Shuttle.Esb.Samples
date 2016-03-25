using System;
using System.Linq;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.Services
{
    public class OrderProcessService : IOrderProcessService
    {
        private readonly IServiceBus _bus;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrderProcessService(IServiceBus bus, IDatabaseContextFactory databaseContextFactory, IOrderProcessViewQuery orderProcessViewQuery)
        {
            Guard.AgainstNull(bus, "bus");
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(orderProcessViewQuery, "orderProcessViewQuery");

            _bus = bus;
            _databaseContextFactory = databaseContextFactory;
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
                var row = _orderProcessViewQuery.Find(id);

                if (row == null)
                {
                    return;
                }

                _bus.Send(new CancelOrderProcessCommand
                {
                    OrderProcessId = id
                }, c =>
                {
                    c.WithRecipient(OrderProcessViewColumns.TargetSystemUri.MapFrom(row));
                    c.WithCorrelationId(id.ToString("N"));
                    c.Headers.Add(new TransportHeader
                    {
                        Key = "TargetSystem",
                        Value = OrderProcessViewColumns.TargetSystem.MapFrom(row)
                    });
                });

                _orderProcessViewQuery.SaveStatus(id, "Cancelling");
            }
        }

        public void ArchiveOrder(Guid id)
        {
            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                var row = _orderProcessViewQuery.Find(id);

                if (row == null)
                {
                    return;
                }

                _bus.Send(new ArchiveOrderProcessCommand
                {
                    OrderProcessId = id
                }, c =>
                {
                    c.WithRecipient(OrderProcessViewColumns.TargetSystemUri.MapFrom(row));
                    c.WithCorrelationId(id.ToString("N"));
                    c.Headers.Add(new TransportHeader
                    {
                        Key = "TargetSystem",
                        Value = OrderProcessViewColumns.TargetSystem.MapFrom(row)
                    });
                });

                _orderProcessViewQuery.SaveStatus(id, "Archiving");
            }
        }
    }
}