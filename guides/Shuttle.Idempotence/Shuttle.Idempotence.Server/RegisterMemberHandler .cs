using System;
using Shuttle.ESB.Core;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
		public void ProcessMessage(IHandlerContext<RegisterMemberCommand> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}' / message id = '{1}'",
				context.Message.UserName,
				context.TransportMessage.MessageId);
			Console.WriteLine();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}