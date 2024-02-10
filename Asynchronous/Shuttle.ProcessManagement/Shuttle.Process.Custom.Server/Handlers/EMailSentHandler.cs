using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class EMailSentHandler : IMessageHandler<EMailSent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public EMailSentHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
            Guard.AgainstNull(repository, nameof(repository));

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(IHandlerContext<EMailSent> context)
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
                        $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
                }

                var orderProcessStatus = new OrderProcessStatus("EMail Sent");

                orderProcess.AddStatus(orderProcessStatus);

                _repository.AddStatus(orderProcessStatus, orderProcess.Id);
            }

            context.Send(new CompleteOrderProcess
            {
                OrderProcessId = orderProcess.Id
            }, c => c.Local());
        }
    }
}