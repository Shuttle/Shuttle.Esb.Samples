using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Process.Custom.Server.Domain;
using Shuttle.ProcessManagement;
using Shuttle.ProcessManagement.DataAccess;
using Shuttle.ProcessManagement.Messages;

namespace Shuttle.Process.Custom.Server
{
    public class EMailSentHandler : IAsyncMessageHandler<EMailSent>
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

        public async Task ProcessMessageAsync(IHandlerContext<EMailSent> context)
        {
            if (!context.TransportMessage.IsHandledHere())
            {
                return;
            }

            OrderProcess orderProcess;

            using (_databaseContextFactory.Create(ProcessManagementData.ConnectionStringName))
            {
                orderProcess = await _repository.GetAsync(new Guid(context.TransportMessage.CorrelationId));

                if (orderProcess == null)
                {
                    throw new ApplicationException($"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
                }

                var orderProcessStatus = new OrderProcessStatus("EMail Sent");

                orderProcess.AddStatus(orderProcessStatus);

                await _repository.AddStatusAsync(orderProcessStatus, orderProcess.Id);
            }

            await context.SendAsync(new CompleteOrderProcess
            {
                OrderProcessId = orderProcess.Id
            }, c => c.Local());
        }
    }
}