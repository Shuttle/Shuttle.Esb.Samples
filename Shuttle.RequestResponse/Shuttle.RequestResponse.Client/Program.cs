﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Esb;
using Shuttle.Esb.AzureMQ;
using Shuttle.RequestResponse.Messages;

namespace Shuttle.RequestResponse.Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var services = new ServiceCollection();

			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

			services.AddSingleton<IConfiguration>(configuration);

			services.AddServiceBus(builder =>
			{
				configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);
			});

			services.AddAzureStorageQueues(builder =>
			{
				builder.AddConnectionString("azure");
			});

			using (var bus = services.BuildServiceProvider().GetRequiredService<IServiceBus>().Start())
			{
				string userName;

				while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
				{
					bus.Send(new RegisterMemberCommand
					{
						UserName = userName
					}, c => c.WillExpire(DateTime.Now.AddSeconds(5)));
				}
			}
		}
	}
}