using System;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Container;
using Shuttle.Core.Castle;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.ProcessManagement.Services;

namespace Shuttle.ProcessManagement.WebApi
{
    public class Startup
    {
        private IServiceBus _bus;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();

            var windsorContainer = new WindsorContainer();

            var container = new WindsorComponentContainer(windsorContainer);

            container.RegisterSuffixed("Shuttle.ProcessManagement");
            container.Register<IOrderProcessService, OrderProcessService>();

            container.Register<IAzureStorageConfiguration, DefaultAzureStorageConfiguration>();
            container.RegisterServiceBus();

            _bus = container.Resolve<IServiceBus>().Start();

            return WindsorRegistrationHelper.CreateServiceProvider(windsorContainer, services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void OnShutdown()
        {
            _bus?.Dispose();
        }
    }
}