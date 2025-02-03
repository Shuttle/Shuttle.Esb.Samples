using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Server;

public class RegisterMemberHandler : IMessageHandler<RegisterMember>
{
    public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
    {
        Console.WriteLine();
        Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}' / message id = '{context.TransportMessage.MessageId}'");
        Console.WriteLine();

        await Task.CompletedTask;
    }
}