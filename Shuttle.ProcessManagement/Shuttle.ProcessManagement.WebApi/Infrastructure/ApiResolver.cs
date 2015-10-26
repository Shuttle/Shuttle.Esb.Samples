using System;
using System.Collections.Generic;
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
			return _container.Resolve(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			yield return _container.ResolveAll(serviceType);
		}

		public IDependencyScope BeginScope()
		{
			return this;
		}
	}
}