using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi
{
	public class CorsMessageHandler : DelegatingHandler
	{
		private const string Origin = "Origin";
		private const string AccessControlRequestMethod = "Access-Control-Request-Method";
		private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
		private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
		private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
		private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

		private ILog _log;

		public CorsMessageHandler()
		{
			_log = Log.For(this);
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
															   CancellationToken cancellationToken)
		{
			return request.Headers.Contains(Origin)
					   ? ProcessCorsRequest(request, ref cancellationToken)
					   : base.SendAsync(request, cancellationToken);
		}

		private Task<HttpResponseMessage> ProcessCorsRequest(HttpRequestMessage request,
															 ref CancellationToken cancellationToken)
		{
			if (request.Method == HttpMethod.Options)
			{
				return Task.Factory.StartNew(() =>
				{
					var response = new HttpResponseMessage(HttpStatusCode.OK);
					AddCorsResponseHeaders(request, response);
					return response;
				}, cancellationToken);
			}

			return base.SendAsync(request, cancellationToken).ContinueWith(
				task =>
				{
					var resp = task.Result;
					resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
					return resp;
				}, cancellationToken);
		}

		private void AddCorsResponseHeaders(HttpRequestMessage request, HttpResponseMessage response)
		{
			var origin = request.Headers.GetValues(Origin).First();

			response.Headers.Add(AccessControlAllowOrigin, origin);

			var accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
			if (accessControlRequestMethod != null)
			{
				response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
			}

			var requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
			if (!string.IsNullOrEmpty(requestedHeaders))
			{
				response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
			}
		}
	}
}