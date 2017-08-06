using Shuttle.Core.ServiceHost;

namespace Shuttle.RequestResponse.Server
{
    internal class Program
    {
        private static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}