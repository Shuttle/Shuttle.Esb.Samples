using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Server
{
	public class RegisterMemberHandler : IAsyncMessageHandler<RegisterMember>
	{
		public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			await context.SendAsync(new MemberRegistered
			{
				UserName = context.Message.UserName
			}, builder => builder.Reply());
		}
	}
}