using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class RegisterOrderProcessHandler : IMessageHandler<RegisterOrderProcess>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public RegisterOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(IHandlerContext<RegisterOrderProcess> context)
        {
            var message = context.Message;

            var orderProcess = new OrderProcess
            {
                OrderId = null,
                InvoiceId = null,
                CustomerName = message.CustomerName,
                CustomerEMail = message.CustomerEMail,
                TargetSystem = message.TargetSystem,
                TargetSystemUri = message.TargetSystemUri
            };

            orderProcess.GenerateOrderNumber();

            foreach (var quotedProduct in message.QuotedProducts)
            {
                orderProcess.AddItem(new OrderProcessItem(quotedProduct.ProductId, quotedProduct.Description,
                    quotedProduct.Price));
            }

            var status = new OrderProcessStatus("Cooling Off");

            orderProcess.AddStatus(status);

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _repository.Add(orderProcess);
            }

            context.Publish(new OrderProcessRegistered
            {
                OrderProcessId = orderProcess.Id,
                QuotedProducts = message.QuotedProducts,
                CustomerName = message.CustomerName,
                CustomerEMail = message.CustomerEMail,
                OrderNumber = orderProcess.OrderNumber,
                OrderDate = orderProcess.DateRegistered,
                OrderTotal = orderProcess.Total(),
                Status = status.Status,
                StatusDate = status.StatusDate,
                TargetSystem = message.TargetSystem,
                TargetSystemUri = message.TargetSystemUri
            });

            context.Send(new AcceptOrderProcess
            {
                OrderProcessId = orderProcess.Id
            }, c => c.Defer(DateTime.Now.AddSeconds(10)).Local());
        }
    }
}