using System;
using System.Collections.Generic;

namespace Shuttle.ProcessManagement.WebApi
{
	public class RegisterOrderModel
	{
		public List<string> ProductIds { get; set; }
		public string TargetSystem { get; set; }
	}
}