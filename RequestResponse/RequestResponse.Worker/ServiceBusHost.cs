using System;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace RequestResponse.Worker
{
	public class ServiceBusHost : IHost, IDisposable
	{
		private IServiceBus bus;

		public void Dispose()
		{
			bus.Dispose();
		}

		public void Start()
		{
			Log.Assign(new ConsoleLog(typeof (ServiceBusHost)) {LogLevel = LogLevel.Trace});

			bus = ServiceBus
				.Create(c => c.AddEnryptionAlgorithm(new TripleDesEncryptionAlgorithm())
				              .AddCompressionAlgorithm(new GZipCompressionAlgorithm())
				)
				.Start();
		}
	}
}