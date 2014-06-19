using System;
using PublishSubscribe.Messages;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace PublishSubscribe.Subscriber2
{
    public class Subscriber2Handler : IMessageHandler<OrderCompletedEvent>
    {
		public void ProcessMessage(HandlerContext<OrderCompletedEvent> context)
    	{
			var comment = string.Format("Handled OrderCompletedEvent on Subscriber2: {0}", context.Message.OrderId);

			ColoredConsole.WriteLine(ConsoleColor.Blue, comment);

			context.Publish(new WorkDoneEvent
			{
				Comment = comment
			});
			
			context.Send(new WorkDoneEvent
			{
				Comment = "[DEFERRED / Subscriber2] : order id = " + context.Message.OrderId
			}, c => c.Defer(DateTime.Now.AddSeconds(5)).Reply());
		}

    	public bool IsReusable
    	{
    		get { return true; }
    	}
    }
}