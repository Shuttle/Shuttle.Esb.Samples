﻿using System.Threading;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;

namespace Shuttle.EMailSender.Server
{
    public class SendEMailHandler : IMessageHandler<SendEMailCommand>
    {
        public bool IsReusable => true;

        public void ProcessMessage(IHandlerContext<SendEMailCommand> context)
        {
            // simulate sending an e-mail

            Thread.Sleep(2000);

            context.Publish(new EMailSentEvent
            {
                EMailId = context.Message.EMailId
            });
        }
    }
}