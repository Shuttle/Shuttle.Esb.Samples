using Shuttle.Core.WorkerService;

namespace Shuttle.Distribution.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}