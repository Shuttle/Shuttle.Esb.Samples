using System.Web.Http;
using System.Web.Http.Cors;

namespace Shuttle.ProcessManagement.WebApi
{
    public static class WebApiConfiguration
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            configuration.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional});

            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}