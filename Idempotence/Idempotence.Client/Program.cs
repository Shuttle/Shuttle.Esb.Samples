using System;
using Idempotence.Messages;
using log4net;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;

namespace Idempotence.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (Program))));

			using (var bus = ServiceBus.Create().Start())
			{
				string text;

				Console.WriteLine("Type a message to send to the server or an empty line to exit:");

				for (var i = 0; i < 1000; i++)
				{
					bus.Send(new ShowMessageCommand { Text = Guid.NewGuid().ToString() });
				}

				while (!string.IsNullOrEmpty(text = Console.ReadLine()))
				{
					bus.Send(new ShowMessageCommand {Text = text});
				}
			}
		}
	}
}