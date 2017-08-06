using Shuttle.Core.ServiceHost;

namespace Shuttle.Process.Custom.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}