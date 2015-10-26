using System.Collections.Generic;
using System.Web.Http;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
	public class OrdersController : ApiController
	{
		// GET api/values
		public IEnumerable<string> Get()
		{
			return new[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post([FromBody] RegisterOrderModel model)
		{
			Guard.AgainstNull(model, "model");
		}

		// PUT api/values/5
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}