using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.Distribution.Messages;

namespace Shuttle.Distribution.Worker
{
	public class RegisterMemberHandler : IAsyncMessageHandler<RegisterMember>
	{
		public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED --- WORKER] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			await Task.CompletedTask;
		}
	}
}