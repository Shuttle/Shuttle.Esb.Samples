using System;
using System.Threading.Tasks;

namespace Shuttle.DependencyInjection.EMail
{
	public class EMailService : IEMailService
	{
		public async Task SendAsync(string name)
		{
			Console.WriteLine();
			Console.WriteLine("[SENDING E-MAIL] : name = '{0}'", name);
			Console.WriteLine();

            await Task.Delay(TimeSpan.FromSeconds(3)); // simulate communication wait time

			Console.WriteLine();
			Console.WriteLine("[E-MAIL SENT] : name = '{0}'", name);
			Console.WriteLine();
        }
	}
}