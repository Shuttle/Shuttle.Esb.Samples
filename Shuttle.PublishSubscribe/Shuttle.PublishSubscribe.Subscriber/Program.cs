using Shuttle.Core.ServiceHost;

namespace Shuttle.PublishSubscribe.Subscriber
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}