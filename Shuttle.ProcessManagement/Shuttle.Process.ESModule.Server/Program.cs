using Shuttle.Core.ServiceHost;

namespace Shuttle.Process.ESModule.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}