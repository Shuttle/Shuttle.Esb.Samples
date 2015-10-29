using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Castle;
using Shuttle.ESB.Core;
using Shuttle.ESB.SqlServer;
using Shuttle.ProcessManagement;

namespace Shuttle.Process.HandRolled.Server
{
    public class Host : IHost, IDisposable
    {
        private IServiceBus _bus;
        private WindsorContainer _container;

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void Start()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (Host))));

            _container = new WindsorContainer();

            _container.RegisterDataAccess();

            // register all the message handlers in this assembly
            _container.Register(
                Classes.FromThisAssembly()
                .BasedOn(typeof(IMessageHandler<>))
                .WithServiceFromInterface(typeof(IMessageHandler<>))
                .LifestyleTransient()
                );

            _bus = ServiceBus.Create(
                c => c
                    .MessageHandlerFactory(new CastleMessageHandlerFactory(_container))
                    .SubscriptionManager(SubscriptionManager.Default())
                ).Start();
        }
    }
}