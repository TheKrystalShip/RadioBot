using Discord;
using Discord.WebSocket;

using RadioBot.Handlers;
using RadioBot.Properties;

using System.Threading.Tasks;

namespace RadioBot
{
	public class Program
    {
		private string Token;
		private DiscordSocketClient Client;
		private CommandHandler CommandHanlder;
		private EventHandler EventHandler;

		static void Main(string[] args)
			=> new Program().Init().Wait();

		private async Task Init()
		{
			Token = Resources.Token;

			Client = new DiscordSocketClient(new DiscordSocketConfig() {
				DefaultRetryMode = RetryMode.AlwaysRetry,
				ConnectionTimeout = 5000,
				LogLevel = LogSeverity.Debug,
				AlwaysDownloadUsers = true
			});

			CommandHanlder = new CommandHandler(Client);
			EventHandler = new EventHandler(Client);

			await Client.LoginAsync(TokenType.Bot, Token);
			await Client.StartAsync();
			await Client.SetGameAsync("music 🎶");

			await Task.Delay(-1);
		}
    }
}
