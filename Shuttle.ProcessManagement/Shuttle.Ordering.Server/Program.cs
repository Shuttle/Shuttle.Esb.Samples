using Shuttle.Core.ServiceHost;

namespace Shuttle.Ordering.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}