using Shuttle.Core.WorkerService;

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