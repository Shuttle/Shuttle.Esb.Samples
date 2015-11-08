using System;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public class ProcessMessageHandlerInvoker : IMessageHandlerInvoker
    {
        private readonly Type _eventStreamType = typeof(EventStream);
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IMessageHandlerInvoker _defaultMessageHandlerInvoker;
        private readonly IEventStore _eventStore;
        private readonly IProcessConfiguration _configuration;

        public ProcessMessageHandlerInvoker(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IProcessConfiguration configuration)
        {
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(eventStore, "eventStore");
            Guard.AgainstNull(configuration, "configuration");

            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _configuration = configuration;

            _defaultMessageHandlerInvoker = new DefaultMessageHandlerInvoker();
        }

        public MessageHandlerInvokeResult Invoke(PipelineEvent pipelineEvent)
        {
            var state = pipelineEvent.Pipeline.State;
            var bus = state.GetServiceBus();
            var transportMessage = state.GetTransportMessage();
            var processAssemblyQualifiedName = transportMessage.GetProcessAssemblyQualifiedName();
            Guid processInstanceId;


            if (string.IsNullOrEmpty(processAssemblyQualifiedName) ||
                !Guid.TryParse(transportMessage.CorrelationId, out processInstanceId))
            {
                return _defaultMessageHandlerInvoker.Invoke(pipelineEvent);
            }

            var processType = Type.GetType(processAssemblyQualifiedName, true, true);
            object processInstance;

            try
            {
                processInstance = Activator.CreateInstance(processType, processInstanceId);
            }
            catch
            {
                throw new ProcessException(string.Format(ProcessResources.MissingProcessConstructor,
                    processAssemblyQualifiedName));
            }

            EventStream stream;

            using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
            {
                stream = _eventStore.Get(processInstanceId);
            }

            stream.Apply(processInstance);

            var message = state.GetMessage();
            var messageType = message.GetType();
            var contextType = typeof(HandlerContext<>).MakeGenericType(messageType);
            var method = processType.GetMethod("ProcessMessage", new[] { contextType, _eventStreamType });

            if (method == null)
            {
                throw new ProcessMessageMethodMissingException(string.Format(
                    ESBResources.ProcessMessageMethodMissingException,
                    processInstance.GetType().FullName,
                    messageType.FullName));
            }

            var handlerContext = Activator.CreateInstance(contextType, bus, transportMessage, message, state.GetActiveState());

            method.Invoke(processInstance, new[] { handlerContext, stream });

            using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
            {
                _eventStore.SaveEventStream(stream);
            }

            return MessageHandlerInvokeResult.InvokedHandler(processInstance);
        }
    }
}