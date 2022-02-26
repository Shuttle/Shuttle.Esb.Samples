using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Shuttle.Core.WorkerService;

namespace Shuttle.Invoicing.Server
{
    public class Program
    {
        public static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            ServiceHost.Run<Host>();
        }
    }
}