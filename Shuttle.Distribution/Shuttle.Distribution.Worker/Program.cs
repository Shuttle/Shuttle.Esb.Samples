using Shuttle.Core.ServiceHost;

namespace Shuttle.Distribution.Worker
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}