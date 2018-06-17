using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using RadioBot.Services;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RadioBot.Handlers
{
	public class CommandHandler
    {
		private DiscordSocketClient Client;
		private CommandService CommandService;
		private readonly IServiceProvider ServiceCollection;

		public CommandHandler(DiscordSocketClient client)
		{
			Client = client;

			CommandService = new CommandService(new CommandServiceConfig()
				{
					CaseSensitiveCommands = false,
					DefaultRunMode = RunMode.Async
				}
			);

			CommandService.AddModulesAsync(Assembly.GetEntryAssembly()).Wait();

			ServiceCollection = new ServiceCollection()
				.AddSingleton(new RadioService())
				.BuildServiceProvider();

			Console.Title = "RadioBot";

			Client.MessageReceived += HandleCommands;
		}

		private async Task HandleCommands(SocketMessage arg)
		{
            SocketUserMessage message = arg as SocketUserMessage;

			if (message is null || message.Author.IsBot)
				return;

			int argPos = 0;
			bool mention = message.HasMentionPrefix(Client.CurrentUser, ref argPos);

			if (mention)
			{
				SocketCommandContext context = new SocketCommandContext(Client, message);
				IResult result = await CommandService.ExecuteAsync(context, argPos, ServiceCollection);

				if (!result.IsSuccess)
				{
					Console.WriteLine(new LogMessage(LogSeverity.Error, "Command", result.ErrorReason));
				}
			}
		}
	}
}
