using System;
using Idempotence.Messages;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Idempotence.Server
{
	public class ShowMessageHandler : IMessageHandler<ShowMessageCommand>
	{
		private readonly ILog _log;

		public ShowMessageHandler()
		{
			_log = Log.For(this);
		}

		public void ProcessMessage(HandlerContext<ShowMessageCommand> context)
		{
			var message = string.Format("[SHOW MESSAGE] : text = '{0}'", context.Message.Text);

			ColoredConsole.WriteLine(ConsoleColor.Blue, message);

			_log.Information(message);

			var response = new MessageShownEvent
			{
				From = typeof(Host).Assembly.GetName().Name,
				Text = context.Message.Text,
				When = DateTime.Now
			};

			context.Send(response, c => c.Reply());
			context.Publish(response);
		}

		public bool IsReusable {
			get { return true; }
		}
	}
}