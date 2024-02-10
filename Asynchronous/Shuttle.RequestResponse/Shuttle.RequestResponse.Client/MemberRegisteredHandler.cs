using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Client
{
	public class MemberRegisteredHandler : IAsyncMessageHandler<MemberRegistered>
	{
		public async Task ProcessMessageAsync(IHandlerContext<MemberRegistered> context)
		{
			Console.WriteLine();
			Console.WriteLine("[RESPONSE RECEIVED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			await Task.CompletedTask;
		}
	}
}