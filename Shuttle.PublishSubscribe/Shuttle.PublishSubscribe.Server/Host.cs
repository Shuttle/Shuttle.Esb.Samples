using System;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.StructureMap;
using Shuttle.Esb;
using Shuttle.Esb.Msmq;
using Shuttle.Esb.Sql;
using StructureMap;

namespace Shuttle.PublishSubscribe.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
			var smRegistry = new Registry();
			var registry = new StructureMapComponentRegistry(smRegistry);

			registry.Register<IMsmqConfiguration, MsmqConfiguration>();
			registry.Register<TransactionScopeObserver>();

			var configurator = new ServiceBusConfigurator(registry);

			configurator.DontRegister<ISubscriptionManager>();

			registry.Register<Esb.Sql.IScriptProvider, Esb.Sql.ScriptProvider>();
			registry.Register<Esb.Sql.IScriptProviderConfiguration, Esb.Sql.ScriptProviderConfiguration>();

			registry.Register<ISqlConfiguration>(SqlSection.Configuration());
			registry.Register<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
			registry.Register<IDatabaseContextFactory, DatabaseContextFactory>();
			registry.Register<IDbConnectionFactory, DbConnectionFactory>();
			registry.Register<IDbCommandFactory, DbCommandFactory>();
			registry.Register<IDatabaseGateway, DatabaseGateway>();
			registry.Register<ISubscriptionManager, SubscriptionManager>();

			configurator.Configure();

			_bus = ServiceBus.Create(new StructureMapComponentResolver(new Container(smRegistry))).Start();
		}

		public void Dispose()
		{
			_bus.Dispose();
		}
	}
}