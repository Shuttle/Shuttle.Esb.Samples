using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Shuttle.Core.Castle;
using Shuttle.Core.Data.Registration;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Esb;
using ILog = Shuttle.Core.Logging.ILog;

namespace Shuttle.ProcessManagement.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Program))));

            Log.Information("[started]");

            BuildWebHost(args).Run();

            Log.Information("[stopped]");
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }

    }
}