using System.Net.Http;
using System.Web.Http;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
    public class ServerController : ShuttleApiController
    {
        [HttpGet]
        [Route("api/server/properties")]
        public HttpResponseMessage Properties()
        {
            return OK(new
            {
                Version = "1.0.0"
            });
        }
    }
}