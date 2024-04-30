using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Pipelines;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Idempotence.Messages;

namespace Shuttle.Idempotence.Client
{
	internal class Program
	{
		private static async Task Main(string[] args)
		{
			var services = new ServiceCollection();

			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

			services.AddSingleton<IConfiguration>(configuration);

			services.AddServiceBus(builder =>
			{
				configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);

				builder.Options.Asynchronous = true;
			});

			services.AddAzureStorageQueues(builder =>
			{
				builder.AddOptions("azure", new AzureStorageQueueOptions
				{
					ConnectionString = configuration.GetConnectionString("azure")
				});
			});

			Console.WriteLine("Type some characters and then press [enter] to submit; an empty line submission stops execution:");
			Console.WriteLine();

			var serviceProvider = services.BuildServiceProvider();
			var pipelineFactory = serviceProvider.GetRequiredService<IPipelineFactory>();
			var messageSender = serviceProvider.GetRequiredService<IMessageSender>();
			var transportMessagePipeline = pipelineFactory.GetPipeline<TransportMessagePipeline>();

			await using (var serviceBus = await serviceProvider.GetRequiredService<IServiceBus>().StartAsync())
			{
				string userName;

				while (!string.IsNullOrEmpty(userName = Console.ReadLine()))
				{
					var command = new RegisterMember
					{
						UserName = userName
					};

					await transportMessagePipeline.ExecuteAsync(command, null, null);

					var transportMessage = transportMessagePipeline.State.GetTransportMessage();

					for (var i = 0; i < 5; i++)
					{
						await messageSender.DispatchAsync(transportMessage, null); // will be processed only once since message id is the same
					}

					await serviceBus.SendAsync(command); // will be processed since it has a new message id
					await serviceBus.SendAsync(command); // will also be processed since it too has a new message id
				}
			}
		}
	}
}