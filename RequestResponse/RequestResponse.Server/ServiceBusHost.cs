using System;
using log4net;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;

namespace RequestResponse.Server
{
	public class ServiceBusHost : IHost, IDisposable
	{
		private static IServiceBus bus;

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(ServiceBusHost))));

			bus = ServiceBus
				.Create()
				.AddEnryptionAlgorithm(new TripleDesEncryptionAlgorithm())
				.AddCompressionAlgorithm(new GZipCompressionAlgorithm())
				.Start();
		}

		public void Dispose()
		{
			bus.Dispose();

			LogManager.Shutdown();
		}
	}
}