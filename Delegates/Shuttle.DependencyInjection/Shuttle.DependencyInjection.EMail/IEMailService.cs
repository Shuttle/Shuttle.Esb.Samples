using System.Threading.Tasks;

namespace Shuttle.DependencyInjection.EMail
{
	public interface IEMailService
	{
		Task SendAsync(string name);
	}
}