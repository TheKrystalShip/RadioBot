using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot.Core
{
    public class Bot : DiscordSocketClient
    {
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

        }

        public async Task InitAsync(string token)
        {
            await LoginAsync(TokenType.Bot, token);
            await StartAsync();
            await SetGameAsync("music 🎶");
        }
    }
}
