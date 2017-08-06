using Shuttle.Core.ServiceHost;

namespace Shuttle.EMailSender.Server
{
    public class Program
    {
        public static void Main()
        {
            ServiceHost.Run<Host>();
        }
    }
}