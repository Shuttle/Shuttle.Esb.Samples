using System;
using Idempotence.Messages;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Idempotence.Client
{
	public class MessageShownHandler : IMessageHandler<MessageShownEvent>
	{
		private readonly ILog _log;

		public MessageShownHandler()
		{
			_log = Log.For(this);
		}

		public void ProcessMessage(HandlerContext<MessageShownEvent> context)
		{
			var message = string.Format("[MESSAGE SHOWN] : from = '{0}' / text = '{1}' / when = '{2}'", context.Message.From, context.Message.Text, context.Message.When);

			ColoredConsole.WriteLine(ConsoleColor.Blue, message);

			_log.Information(message);
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}