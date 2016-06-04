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

        public static void RegisterDataAccessCore(this IWindsorContainer container)
        {
            Guard.AgainstNull(container, "container");

            container.Register(Component.For<IDatabaseContextCache>().ImplementedBy<ThreadStaticDatabaseContextCache>());
            container.Register(Component.For<IDatabaseGateway>().ImplementedBy<DatabaseGateway>());
            container.Register(Component.For(typeof(IDataRepository<>)).ImplementedBy(typeof(DataRepository<>)));

            //container.Register(
            //    Classes
            //        .FromAssemblyNamed("Shuttle.Core.Data")
            //        .Pick()
            //        .If(type => type.Name.EndsWith("Factory"))
            //        .Configure(configurer => configurer.Named(configurer.Implementation.Name.ToLower()))
            //        .WithService.Select((type, basetype) => new[] { type.InterfaceMatching(@".*Factory\Z") }));

            container.Register(
                Classes
                    .FromAssemblyNamed("Shuttle.Core.Data")
                    .Pick()
                    .If(type => type.Name.EndsWith("Factory"))
                    .WithServiceFirstInterface());
        }
    }
}