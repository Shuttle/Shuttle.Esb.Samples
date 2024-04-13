using System;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.DataAccess.OrderProcessView;
using Shuttle.ProcessManagement.Domain;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.ProcessManagement.Services
{
    public class OrderProcessService : IOrderProcessService
    {
        private readonly IServiceBus _bus;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessViewQuery _orderProcessViewQuery;

        public OrderProcessService(IServiceBus bus, IDatabaseContextFactory databaseContextFactory,
            IOrderProcessViewQuery orderProcessViewQuery)
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
                    let status = (OrderProcessViewColumns.Status.Value(row) ?? string.Empty).ToLower()
                    select new
                    {
                        Id = OrderProcessViewColumns.Id.Value(row),
                        CustomerName = OrderProcessViewColumns.CustomerName.Value(row),
                        OrderNumber = OrderProcessViewColumns.OrderNumber.Value(row),
                        OrderDate = OrderProcessViewColumns.OrderDate.Value(row),
                        OrderTotal = OrderProcessViewColumns.OrderTotal.Value(row),
                        Status = OrderProcessViewColumns.Status.Value(row),
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

                _bus.Send(new CancelOrderProcess
                {
                    OrderProcessId = id
                }, builder =>
                {
                    builder.WithRecipient(OrderProcessViewColumns.TargetSystemUri.Value(row));
                    builder.WithCorrelationId(id.ToString("N"));
                    builder.Headers.Add(new TransportHeader
                    {
                        Key = "TargetSystem",
                        Value = OrderProcessViewColumns.TargetSystem.Value(row)
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

                _bus.Send(new ArchiveOrderProcess
                {
                    OrderProcessId = id
                }, builder =>
                {
                    builder.WithRecipient(OrderProcessViewColumns.TargetSystemUri.Value(row));
                    builder.WithCorrelationId(id.ToString("N"));
                    builder.Headers.Add(new TransportHeader
                    {
                        Key = "TargetSystem",
                        Value = OrderProcessViewColumns.TargetSystem.Value(row)
                    });
                });

                _orderProcessViewQuery.SaveStatus(id, "Archiving");
            }
        }
    }
}