﻿using System;
using System.Threading.Tasks;
using Shuttle.Esb;
using Shuttle.PublishSubscribe.Messages;

namespace Shuttle.PublishSubscribe.Server;

public class RegisterMemberHandler : IMessageHandler<RegisterMember>
{
    public async Task ProcessMessageAsync(IHandlerContext<RegisterMember> context)
    {
        Console.WriteLine();
        Console.WriteLine($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");
        Console.WriteLine();

        await context.PublishAsync(new MemberRegistered
        {
            UserName = context.Message.UserName
        });
    }
}