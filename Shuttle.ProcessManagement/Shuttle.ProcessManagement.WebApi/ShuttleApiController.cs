using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Shuttle.ProcessManagement.WebApi
{
	public class ShuttleApiController : ApiController
	{
		protected HttpResponseMessage NotFound(string message)
		{
			return Request.CreateResponse(HttpStatusCode.NotFound, message);
		}

		protected HttpResponseMessage OK<T>(T data)
		{
			return Request.CreateResponse(HttpStatusCode.OK, data);
		}

		protected HttpResponseMessage OK()
		{
			return Request.CreateResponse(HttpStatusCode.OK);
		}

		protected HttpResponseMessage Unauthorized()
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized);
		}

		protected HttpResponseMessage Unauthorized(string message)
		{
			return Request.CreateResponse(HttpStatusCode.Unauthorized, message);
		}

		protected HttpResponseMessage InternalServerError(string message)
		{
			return Request.CreateResponse(HttpStatusCode.InternalServerError, message);
		}
	}
}