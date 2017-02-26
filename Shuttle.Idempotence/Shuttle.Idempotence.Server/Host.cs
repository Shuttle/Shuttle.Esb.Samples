using System;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.SimpleInjector;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;
using Shuttle.Esb.Sql;
using SimpleInjector;

namespace Shuttle.Idempotence.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
            var container = new SimpleInjectorComponentContainer(new Container());

			container.Register<IMsmqConfiguration, MsmqConfiguration>();

				var configurator = new ServiceBusConfigurator(container);

			configurator.DontRegister<IIdempotenceService>();

			container.Register<Esb.Sql.IScriptProvider, Esb.Sql.ScriptProvider>();
			container.Register<Esb.Sql.IScriptProviderConfiguration, Esb.Sql.ScriptProviderConfiguration>();

			container.Register<ISqlConfiguration>(SqlSection.Configuration());
			container.Register<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
            container.Register<IDatabaseContextFactory, DatabaseContextFactory>();
            container.Register<IDbConnectionFactory, DbConnectionFactory>();
            container.Register<IDbCommandFactory, DbCommandFactory>();
            container.Register<IDatabaseGateway, DatabaseGateway>();
            container.Register<IIdempotenceService, IdempotenceService>();

            configurator.Configure();

            _bus = ServiceBus.Create(container).Start();
        }

        public void Dispose()
		{
			_bus.Dispose();
		}
	}
}