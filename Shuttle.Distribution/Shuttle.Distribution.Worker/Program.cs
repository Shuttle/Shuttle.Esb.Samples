using Shuttle.Core.WorkerService;

namespace Shuttle.Distribution.Worker
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}