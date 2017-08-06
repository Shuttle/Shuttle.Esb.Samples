using Shuttle.Core.ServiceHost;

namespace Shuttle.DependencyInjection.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}