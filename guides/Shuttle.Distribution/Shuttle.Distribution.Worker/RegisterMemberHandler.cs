using System;
using Shuttle.ESB.Core;
using Shuttle.Distribution.Messages;

namespace Shuttle.Distribution.Worker
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
		public void ProcessMessage(IHandlerContext<RegisterMemberCommand> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED --- WORKER] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}