using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement
{
    public static class WindsorContainerExtensions
    {
        public static void RegisterDataAccess(this IWindsorContainer container)
        {
            Guard.AgainstNull(container, "container");

            container.Register(Component.For<IDatabaseContextCache>().ImplementedBy<ThreadStaticDatabaseContextCache>());
            container.Register(Component.For<IDatabaseGateway>().ImplementedBy<DatabaseGateway>());
            container.Register(Component.For(typeof(IDataRepository<>)).ImplementedBy(typeof(DataRepository<>)));

            container.Register(
                Classes
                    .FromAssemblyNamed("Shuttle.Core.Data")
                    .Pick()
                    .If(type => type.Name.EndsWith("Factory"))
                    .Configure(configurer => configurer.Named(configurer.Implementation.Name.ToLower()))
                    .WithService.Select((type, basetype) => new[] { type.InterfaceMatching(@".*Factory\Z") }));

            container.Register(
                Classes
                    .FromThisAssembly()
                    .BasedOn(typeof(IDataRowMapper<>))
                    .WithServiceFirstInterface());

            container.Register(
                Classes
                    .FromThisAssembly()
                    .Pick()
                    .If(type => type.Name.EndsWith("Repository"))
                    .WithServiceFirstInterface());

            container.Register(
                Classes
                    .FromThisAssembly()
                    .Pick()
                    .If(type => type.Name.EndsWith("Query"))
                    .WithServiceFirstInterface());

            container.Register(
                Classes
                    .FromThisAssembly()
                    .Pick()
                    .If(type => type.Name.EndsWith("Factory"))
                    .WithServiceFirstInterface());
        }
    }
}