using TheKrystalShip.RadioBot.Core;

namespace TheKrystalShip.RadioBot
{
    public class Program
    {
        public static async Task Main()
        {
            await new Startup()
                .ConfigureServices()
                .ConfigureCommands()
                .ConfigureClient()
                .InitAsync();
        }
    }
}
