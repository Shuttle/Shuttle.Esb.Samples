using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.Windsor;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi
{
    public class ApiResolver : IDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public ApiResolver(IWindsorContainer container)
        {
            Guard.AgainstNull(container, "container");

            _container = container;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return _container.Kernel.HasComponent(serviceType) ? _container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<IEnumerable<object>>();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}