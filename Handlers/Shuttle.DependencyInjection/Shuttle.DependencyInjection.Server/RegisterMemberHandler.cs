using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.DependencyInjection.EMail;
using Shuttle.DependencyInjection.Messages;
using Shuttle.Esb;

namespace Shuttle.DependencyInjection.Server;

public class RegisterMemberHandler : IMessageHandler<RegisterMember>
{
    private readonly IEMailService _emailService;

    public RegisterMemberHandler(IEMailService emailService)
    {
        _emailService = Guard.AgainstNull(emailService);
    }

    public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
    {
        Console.WriteLine();
        Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");
        Console.WriteLine();

        await _emailService.SendAsync(context.Message.UserName);

        await Task.CompletedTask;
    }
}