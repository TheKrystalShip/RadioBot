using Discord;
using Discord.WebSocket;

using RadioBot.Handlers;
using RadioBot.Properties;

using System.Threading.Tasks;

namespace RadioBot
{
	public class Program
    {
		private static string Token;
		private static DiscordSocketClient Client;
		private static CommandHandler CommandHanlder;
		private static EventHandler EventHandler;

		static async Task Main(string[] args)
		{
			Token = Resources.Token;

			Client = new DiscordSocketClient(new DiscordSocketConfig()
				{
					DefaultRetryMode = RetryMode.AlwaysRetry,
					ConnectionTimeout = 5000
				}
			);

			CommandHanlder = new CommandHandler(Client);
			EventHandler = new EventHandler(Client);

			await Client.LoginAsync(TokenType.Bot, Token);
			await Client.StartAsync();
			await Client.SetGameAsync("music 🎶");

			await Task.Delay(-1);
		}
    }
}
