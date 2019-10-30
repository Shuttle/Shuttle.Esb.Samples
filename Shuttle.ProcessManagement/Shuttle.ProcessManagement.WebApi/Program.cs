using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;

namespace Shuttle.ProcessManagement.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Program))));

            Log.Information("[started]");

            BuildWebHost(args).Run();

            Log.Information("[stopped]");
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddCommandLine(args).Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .Build();
        }
    }
}