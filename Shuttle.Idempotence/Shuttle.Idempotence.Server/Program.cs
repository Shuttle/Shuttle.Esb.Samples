using Shuttle.Core.ServiceHost;

namespace Shuttle.Idempotence.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}