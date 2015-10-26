using System.Web;
using System.Web.Mvc;

namespace Shuttle.ProcessManagement.WebApi
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}