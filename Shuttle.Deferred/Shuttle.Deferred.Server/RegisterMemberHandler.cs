using System;
using Shuttle.Esb;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
	    public void ProcessMessage(IHandlerContext<RegisterMemberCommand> context)
		{
		    Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");
		}
	}
}