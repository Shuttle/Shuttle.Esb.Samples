using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Castle
{
    public static class WindsorContainerExtensions
    {
        public static void RegisterDataAccess(this IWindsorContainer container, string assemblyName)
        {
            Guard.AgainstNull(container, "container");

            container.RegisterDataAccess(Assembly.Load(assemblyName));
        }

        public static void RegisterDataAccess(this IWindsorContainer container, Assembly assembly)
        {
            Guard.AgainstNull(container, "container");

            container.Register(
                Classes
                    .FromAssembly(assembly)
                    .BasedOn(typeof(IDataRowMapper<>))
                    .WithServiceFirstInterface());

            container.Register(
                Classes
                    .FromAssembly(assembly)
                    .Pick()
                    .If(type => type.Name.EndsWith("Repository"))
                    .WithServiceFirstInterface());

            container.Register(
                Classes
                    .FromAssembly(assembly)
                    .Pick()
                    .If(type => type.Name.EndsWith("Query"))
                    .WithServiceFirstInterface());

            container.Register(
                Classes
                    .FromAssembly(assembly)
                    .Pick()
                    .If(type => type.Name.EndsWith("Factory"))
                    .WithServiceFirstInterface());
        }
    }
}