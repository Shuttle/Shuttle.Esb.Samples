using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.DependencyInjection.EMail;
using Shuttle.Esb;
using Shuttle.DependencyInjection.Messages;

namespace Shuttle.DependencyInjection.Server
{
	public class RegisterMemberHandler : IAsyncMessageHandler<RegisterMember>
	{
		private readonly IEMailService _emailService;

		public RegisterMemberHandler(IEMailService emailService)
		{
			Guard.AgainstNull(emailService, nameof(emailService));

			_emailService = emailService;
		}

		public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			_emailService.Send(context.Message.UserName);

			await Task.CompletedTask;
		}
	}
}
