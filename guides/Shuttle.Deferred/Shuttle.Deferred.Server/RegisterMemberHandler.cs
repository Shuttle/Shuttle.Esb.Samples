using System;
using Shuttle.ESB.Core;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
		public void ProcessMessage(IHandlerContext<RegisterMemberCommand> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}