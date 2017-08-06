using Shuttle.Core.ServiceHost;

namespace Shuttle.Process.QueryServer
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}