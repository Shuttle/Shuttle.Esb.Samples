using System;

namespace Shuttle.EMailSender.Messages
{
    public class SendEMail
    {
        public Guid EMailId { get; set; }
        public string RecipientEMail { get; set; }
        public string HtmlBody { get; set; }
    }
}