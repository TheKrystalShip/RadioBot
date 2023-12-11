using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;

using TheKrystalShip.DependencyInjection;

namespace TheKrystalShip.RadioBot.Core
{
    public class Bot : DiscordSocketClient
    {
        private readonly CommandHandler _commandHandler;

        private static readonly DiscordSocketConfig __defaultDiscordSocketConfig = new()
        {
            LogLevel = LogSeverity.Info,
            DefaultRetryMode = RetryMode.Retry502
        };

        public Bot() : this(__defaultDiscordSocketConfig)
        {

        }

        public Bot(DiscordSocketConfig config) : base(config)
        {
            _commandHandler = Container.Get<CommandHandler>();
            _commandHandler.SetClient(this);
        }

        public async Task InitAsync(string token)
        {
            await LoginAsync(TokenType.Bot, token);
            await StartAsync();
            await SetGameAsync("music 🎶");
        }
    }
}
