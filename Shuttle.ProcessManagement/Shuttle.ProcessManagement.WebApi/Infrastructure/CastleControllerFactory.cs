using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi
{
	public class CastleControllerFactory : DefaultControllerFactory
	{
		private readonly IWindsorContainer _container;

		public CastleControllerFactory(IWindsorContainer container)
		{
			Guard.AgainstNull(container, "container");

			_container = container;
		}

		public override IController CreateController(RequestContext requestContext, string controllerName)
		{
			try
			{
				return (IController)_container.Resolve(GetControllerType(requestContext, controllerName));
			}
			catch
			{
				return base.CreateController(requestContext, controllerName);
			}
		}

		public override void ReleaseController(IController controller)
		{
			_container.Release(controller);

			base.ReleaseController(controller);
		}
	}
}