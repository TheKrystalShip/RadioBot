using RadioBot.Core;

using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await BotBuilder
                .UseStartup<Startup>()
                .ConfigureServices()
                .ConfigureCommands()
                .ConfigureClient()
                .InitAsync();
        }
    }
}
