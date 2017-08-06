using Shuttle.Core.ServiceHost;

namespace Shuttle.Invoicing.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}