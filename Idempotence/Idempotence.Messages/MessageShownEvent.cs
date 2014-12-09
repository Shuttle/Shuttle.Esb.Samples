using System;

namespace Idempotence.Messages
{
	public class MessageShownEvent
	{
		public string From { get; set; }
		public string Text { get; set; }
		public DateTime When { get; set; }
	}
}