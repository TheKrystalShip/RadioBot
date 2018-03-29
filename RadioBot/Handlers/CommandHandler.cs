using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using RadioBot.Database;
using RadioBot.Services;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RadioBot.Handlers
{
	public class CommandHandler : Handler
    {
		private DiscordSocketClient Client;
		private CommandService CommandService;
		private IServiceProvider ServiceCollection;

		public CommandHandler(DiscordSocketClient client)
		{
			Client = client;

			CommandService = new CommandService();
			CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

			ServiceCollection = new ServiceCollection()
				.AddSingleton(new RadioService())
				.AddDbContext<DatabaseContext>()
				.BuildServiceProvider();

			Console.Title = "RadioBot";

			Client.MessageReceived += HandleCommands;
		}

		private async Task HandleCommands(SocketMessage arg)
		{
			var message = arg as SocketUserMessage;

			if (message is null || message.Author.IsBot)
				return;

			int argPos = 0;
			bool mention = message.HasMentionPrefix(Client.CurrentUser, ref argPos);

			if (mention)
			{
				SocketCommandContext context = new SocketCommandContext(Client, message);
				var result = await CommandService.ExecuteAsync(context, argPos, ServiceCollection);

				if (!result.IsSuccess)
				{
					Console.WriteLine(result);
				}
			}
		}
	}
}
