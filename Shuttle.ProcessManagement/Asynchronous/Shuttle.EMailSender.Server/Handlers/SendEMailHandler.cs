using System.Threading.Tasks;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;

namespace Shuttle.EMailSender.Server
{
    public class SendEMailHandler : IAsyncMessageHandler<SendEMail>
    {
        public async Task ProcessMessageAsync(IHandlerContext<SendEMail> context)
        {
            // simulate sending an e-mail
            await Task.Delay(2000);

            await context.PublishAsync(new EMailSent
            {
                EMailId = context.Message.EMailId
            });
        }
    }
}