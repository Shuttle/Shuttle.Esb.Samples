using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.HandRolled.Server
{
    public class RegisterOrderProcessHandler : IMessageHandler<RegisterOrderProcessCommand>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public RegisterOrderProcessHandler(IDatabaseContextFactory databaseContextFactory,
            IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(repository, "repository");

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(HandlerContext<RegisterOrderProcessCommand> context)
        {
            var orderProcess = new OrderProcess
            {
                OrderId = null,
                InvoiceId = null,
                CustomerName = context.Message.CustomerName,
                CustomerEMail = context.Message.CustomerEMail
            };

            foreach (var quotedProduct in context.Message.QuotedProducts)
            {
                orderProcess.AddItem(new OrderProcessItem(quotedProduct.ProductId, quotedProduct.Description, quotedProduct.Price));
            }

            var status = new OrderProcessStatus("Cooling Off");

            orderProcess.AddStatus(status);

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                _repository.Add(orderProcess);
            }

            context.Publish(new OrderProcessRegisteredEvent
            {
                OrderProcessId = orderProcess.Id,
                QuotedProducts = context.Message.QuotedProducts,
                CustomerName = context.Message.CustomerName,
                CustomerEMail = context.Message.CustomerEMail,
                Status = status.Status,
                StatusDate = status.StatusDate,
                TargetSystem = context.Message.TargetSystem,
                TargetSystemUri = context.Message.TargetSystemUri
            });
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}