using System;
using System.Threading;

namespace Shuttle.DependencyInjection.EMail
{
	public class EMailService : IEMailService
	{
		public void Send(string name)
		{
			Console.WriteLine();
			Console.WriteLine("[SENDING E-MAIL] : name = '{0}'", name);
			Console.WriteLine();

			Thread.Sleep(3000); // simulate communication wait time

			Console.WriteLine();
			Console.WriteLine("[E-MAIL SENT] : name = '{0}'", name);
			Console.WriteLine();
		}
	}
}