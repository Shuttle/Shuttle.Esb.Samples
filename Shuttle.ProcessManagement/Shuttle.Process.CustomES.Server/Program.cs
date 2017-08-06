using Shuttle.Core.ServiceHost;

namespace Shuttle.Process.CustomES.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}