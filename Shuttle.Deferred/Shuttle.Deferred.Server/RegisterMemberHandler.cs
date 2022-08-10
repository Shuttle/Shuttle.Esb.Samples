using System;
using Shuttle.Esb;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMember>
	{
	    public void ProcessMessage(IHandlerContext<RegisterMember> context)
		{
		    Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");
		}
	}
}