using System.Threading;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;

namespace Shuttle.EMailSender.Server
{
    public class SendEMailHandler : IMessageHandler<SendEMail>
    {
        public void ProcessMessage(IHandlerContext<SendEMail> context)
        {
            // simulate sending an e-mail

            Thread.Sleep(2000);

            context.Publish(new EMailSent
            {
                EMailId = context.Message.EMailId
            });
        }
    }
}