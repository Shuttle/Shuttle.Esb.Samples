using System;

namespace Shuttle.EMailSender.Messages
{
    public class EMailSentEvent
    {
        public Guid EMailId { get; set; }
    }
}