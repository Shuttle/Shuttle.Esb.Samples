using System;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.Windsor;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi
{
	public class ShuttleApiControllerActivator : IHttpControllerActivator
	{
		private readonly IWindsorContainer _container;

		public ShuttleApiControllerActivator(IWindsorContainer container)
		{
			Guard.AgainstNull(container, "container");

			_container = container;
		}

		public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor,
									  Type controllerType)
		{
			Guard.AgainstNull(controllerType, "controllerType");

			try
			{
				return _container.Resolve<IHttpController>(controllerType.Name);
			}
			catch
			{
				throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", request.RequestUri.AbsolutePath));
			}
		}
		 
	}
}