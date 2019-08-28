using System.Data.Common;
using System.Data.SqlClient;
using Shuttle.Core.ServiceHost;

namespace Shuttle.Process.CustomES.Server
{
    public class Program
    {
        public static void Main()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            ServiceHost.Run<Host>();
        }
    }
}