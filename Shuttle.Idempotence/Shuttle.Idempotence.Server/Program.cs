using System.Data.Common;
using Microsoft.Data.SqlClient;
using Shuttle.Core.WorkerService;

namespace Shuttle.Idempotence.Server
{
    public class Program
    {
        public static void Main()
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

            ServiceHost.Run<Host>();
        }
    }
}