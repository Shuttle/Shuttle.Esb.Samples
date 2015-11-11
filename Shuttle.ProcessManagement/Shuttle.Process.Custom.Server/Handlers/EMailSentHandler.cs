using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.EMailSender.Messages;
using Shuttle.ESB.Core;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class EMailSentHandler : IMessageHandler<EMailSentEvent>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IOrderProcessRepository _repository;

        public EMailSentHandler(IDatabaseContextFactory databaseContextFactory, IOrderProcessRepository repository)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(repository, "repository");

            _databaseContextFactory = databaseContextFactory;
            _repository = repository;
        }

        public void ProcessMessage(HandlerContext<EMailSentEvent> context)
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

                var orderProcessStatus = new OrderProcessStatus("EMail Sent");

                orderProcess.AddStatus(orderProcessStatus);

                _repository.AddStatus(orderProcessStatus, orderProcess.Id);
            }

            context.Send(new CompleteOrderProcessCommand
            {
                OrderProcessId = orderProcess.Id
            }, c => c.Local());
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}