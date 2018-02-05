﻿using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Invoicing.Messages;
using Shuttle.Ordering.Messages;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.Custom.Server
{
    public class OrderCreatedHandler : IMessageHandler<OrderCreatedEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public OrderCreatedHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<OrderCreatedEvent> context)
        {
            if (!context.TransportMessage.IsHandledHere())
            {
                return;
            }

            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = _repository.Get(new Guid(context.TransportMessage.CorrelationId));

                if (orderProcess == null)
                {
                    throw new ApplicationException(
                        string.Format("Could not find an order process with correlation id '{0}'.",
                            context.TransportMessage.CorrelationId));
                }

                var orderProcessStatus = new OrderProcessStatus("Order Created");

                orderProcess.AddStatus(orderProcessStatus);

                _repository.SaveOrderId(context.Message.OrderId, orderProcess.Id);
                _repository.AddStatus(orderProcessStatus, orderProcess.Id);
            }

            var createInvoiceCommand = new CreateInvoiceCommand
            {
                OrderId = context.Message.OrderId,
                AccountContactName = orderProcess.CustomerName,
                AccountContactEMail = orderProcess.CustomerEMail
            };

            foreach (var messageOrderItem in context.Message.Items)
            {
                createInvoiceCommand.Items.Add(new MessageInvoiceItem
                {
                    Description = messageOrderItem.Description,
                    Price = messageOrderItem.Price
                });
            }

            context.Send(createInvoiceCommand, c => c.WithCorrelationId(orderProcess.Id.ToString()));
        }
    }
}