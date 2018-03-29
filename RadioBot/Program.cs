using Discord;
using Discord.WebSocket;

using RadioBot.Handlers;
using RadioBot.Properties;

using System.Threading.Tasks;

namespace RadioBot
{
	public class Program
    {
		private DiscordSocketClient Client;
		private string Token;

		private CommandHandler CommandHanlder;
		private EventHandler EventHandler;

		static void Main(string[] args)
			=> new Program().Init().Wait();

		private async Task Init()
		{
			Client = new DiscordSocketClient();
			Token = Resources.Token;

			CommandHanlder = new CommandHandler(Client);
			EventHandler = new EventHandler(Client);

			await Client.LoginAsync(TokenType.Bot, Token);
			await Client.StartAsync();
			await Client.SetGameAsync("@RadioBot hello");

			await Task.Delay(-1);
		}
    }
}
