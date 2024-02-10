using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Server
{
	public class RegisterMemberHandler : IAsyncMessageHandler<RegisterMember>
	{
	    public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
		{
		    Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");

		    await Task.CompletedTask;
		}
	}
}