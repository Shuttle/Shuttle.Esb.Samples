using System;
using log4net;
using RequestResponse.Messages;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;

namespace RequestResponse.Client
{
	internal class Program
	{
		private static void Main()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Program))));

			using (var bus = ServiceBus
				.Create(c => c.AddCompressionAlgorithm(new GZipCompressionAlgorithm())
					.AddEnryptionAlgorithm(new TripleDesEncryptionAlgorithm()))
				.Start())
			{
				Console.WriteLine("Client bus started.  Press CTRL+C to stop.");
				Console.WriteLine();
				Console.WriteLine("Press enter to send a message with random text to the server for reversal.");
				Console.WriteLine();

				while (true)
				{
					var input = Console.ReadLine();

					if (!string.IsNullOrEmpty(input) && input.Equals("exit", StringComparison.OrdinalIgnoreCase))
					{
						return;
					}

					var command = new ReverseTextCommand
					{
						Text = Guid.NewGuid().ToString().Substring(0, 5)
					};

					Console.WriteLine("Message (id: {0}) sent.  Text: {1}", bus.Send(command, c =>
					{
						c.WithCorrelationId("correlation-id");
						c.Headers.Add(new TransportHeader {Key = "header1", Value = "value1"});
						c.Headers.Add(new TransportHeader {Key = "header2", Value = "value2"});
					}).MessageId, command.Text);
				}
			}
		}
	}
}