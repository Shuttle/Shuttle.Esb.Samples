using System;
using System.Threading.Tasks;
using Shuttle.Deferred.Messages;
using Shuttle.Esb;

namespace Shuttle.Deferred.Server;

public class RegisterMemberHandler : IMessageHandler<RegisterMember>
{
    public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
    {
        Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");

        await Task.CompletedTask;
    }
}