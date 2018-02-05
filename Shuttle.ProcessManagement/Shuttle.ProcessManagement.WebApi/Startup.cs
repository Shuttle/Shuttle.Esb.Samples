using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Castle;
using Shuttle.Core.Data.Registration;
using Shuttle.Esb;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var container = new WindsorContainer();

            var componentContainer = new WindsorComponentContainer(container);

            componentContainer.RegisterDataAccess("Shuttle.ProcessManagement");

            ServiceBus.Register(componentContainer);

            _bus = ServiceBus.Create(componentContainer).Start();

            //var shuttleApiControllerType = typeof(ShuttleApiController);

            //container.Register(
            //    Classes
            //        .FromThisAssembly()
            //        .Pick()
            //        .If(type => type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            //                    &&
            //                    shuttleApiControllerType.IsAssignableFrom(type))
            //        .LifestyleTransient()
            //        .WithServiceFirstInterface()
            //        .Configure(c => c.Named(c.Implementation.UnderlyingSystemType.Name)));

            return WindsorRegistrationHelper.CreateServiceProvider(container, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}