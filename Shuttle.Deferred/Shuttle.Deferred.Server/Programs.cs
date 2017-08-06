using Shuttle.Core.ServiceHost;

namespace Shuttle.Deferred.Server
{
    public class Programs
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}