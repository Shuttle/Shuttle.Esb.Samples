using System;
using Shuttle.ESB.Core;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Client
{
	public class MemberRegisteredHandler : IMessageHandler<MemberRegisteredEvent>
	{
		public void ProcessMessage(HandlerContext<MemberRegisteredEvent> context)
		{
			Console.WriteLine();
			Console.WriteLine("[RESPONSE RECEIVED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();
		}

		public bool IsReusable {
			get { return true; } 
		}
	}
}