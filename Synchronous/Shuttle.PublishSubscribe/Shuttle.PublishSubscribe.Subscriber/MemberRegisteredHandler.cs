using System;
using Shuttle.Esb;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Subscriber
{
	public class MemberRegisteredHandler : IMessageHandler<MemberRegistered>
	{
		public void ProcessMessage(IHandlerContext<MemberRegistered> context)
		{
			Console.WriteLine();
			Console.WriteLine("[EVENT RECEIVED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();
		}
	}
}