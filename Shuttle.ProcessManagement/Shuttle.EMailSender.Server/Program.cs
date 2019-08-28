using System;
using System.Data.Common;
using System.Data.SqlClient;
using Shuttle.Core.ServiceHost;

namespace Shuttle.EMailSender.Server
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