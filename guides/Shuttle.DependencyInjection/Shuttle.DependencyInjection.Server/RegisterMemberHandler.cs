using System;
using Shuttle.Core.Infrastructure;
using Shuttle.DependencyInjection.EMail;
using Shuttle.ESB.Core;
using Shuttle.DependencyInjection.Messages;

namespace Shuttle.DependencyInjection.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
		private readonly IEMailService _emailService;

		public RegisterMemberHandler(IEMailService emailService)
		{
			Guard.AgainstNull(emailService, "emailService");

			_emailService = emailService;
		}

		public void ProcessMessage(IHandlerContext<RegisterMemberCommand> context)
		{
			Console.WriteLine();
			Console.WriteLine("[MEMBER REGISTERED] : user name = '{0}'", context.Message.UserName);
			Console.WriteLine();

			_emailService.Send(context.Message.UserName);
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
