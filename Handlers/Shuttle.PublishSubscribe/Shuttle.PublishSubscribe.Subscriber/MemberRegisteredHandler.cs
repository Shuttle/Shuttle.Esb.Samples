using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Subscriber;

public class MemberRegisteredHandler : IMessageHandler<MemberRegistered>
{
    public async Task ProcessMessageAsync(IHandlerContext<MemberRegistered> context)
    {
        Console.WriteLine();
        Console.WriteLine($"[EVENT RECEIVED] : user name = '{context.Message.UserName}'");
        Console.WriteLine();

        await Task.CompletedTask;
    }
}