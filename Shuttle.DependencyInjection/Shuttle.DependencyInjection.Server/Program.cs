using Shuttle.Core.WorkerService;

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