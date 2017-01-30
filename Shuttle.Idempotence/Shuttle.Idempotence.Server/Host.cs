﻿using System;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.SimpleInjector;
using Shuttle.Esb;
using Shuttle.Esb.SqlServer;
using Shuttle.Esb.SqlServer.Idempotence;
using SimpleInjector;

namespace Shuttle.Idempotence.Server
{
	public class Host : IHost, IDisposable
	{
		private IServiceBus _bus;

		public void Start()
		{
            var container = new SimpleInjectorComponentContainer(new Container());

            var configurator = new ServiceBusConfigurator(container);

		    configurator.DontRegister<IIdempotenceService>();

            var sqlServerConfiguration = SqlServerSection.Configuration();

            container.Register<ISqlServerConfiguration>(sqlServerConfiguration);
            container.Register<IScriptProvider>(new ScriptProvider(sqlServerConfiguration.ScriptFolder));
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