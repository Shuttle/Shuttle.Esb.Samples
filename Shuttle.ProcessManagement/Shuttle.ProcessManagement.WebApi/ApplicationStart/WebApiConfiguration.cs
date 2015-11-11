using System.Web.Http;

namespace Shuttle.ProcessManagement.WebApi
{
    public static class WebApiConfiguration
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );
        }
    }
}