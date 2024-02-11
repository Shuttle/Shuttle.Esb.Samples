using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Subscriber
{
	public class MemberRegisteredHandler : IAsyncMessageHandler<MemberRegistered>
	{
		public async Task ProcessMessageAsync(IHandlerContext<MemberRegistered> context)
		{
			Console.WriteLine();
			Console.WriteLine("[EVENT RECEIVED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			await Task.CompletedTask;
		}
	}
}