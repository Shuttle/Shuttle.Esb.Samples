using System;
using Shuttle.ESB.Core;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
		public void ProcessMessage(HandlerContext<RegisterMemberCommand> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			context.Publish(new MemberRegisteredEvent
			{
				UserName = context.Message.UserName
			});
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
