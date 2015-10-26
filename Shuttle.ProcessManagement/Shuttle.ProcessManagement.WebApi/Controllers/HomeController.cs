using System.Web.Mvc;

namespace Shuttle.ProcessManagement.WebApi.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}