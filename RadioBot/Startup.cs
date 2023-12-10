using TheKrystalShip.RadioBot.Core;
using TheKrystalShip.RadioBot.Tools;
using TheKrystalShip.RadioBot.Core.Commands;
using TheKrystalShip.RadioBot.Core.Services;
using TheKrystalShip.DependencyInjection;

namespace TheKrystalShip.RadioBot
{
    public class Startup
    {
        public Startup ConfigureCommands()
        {
            Container.Add<CommandHandler>();
            // Call to instantiate
            Container.Get<CommandHandler>();

            return this;
        }

        public Startup ConfigureServices()
        {
            Container.Add<IRadioService, RadioService>();

            return this;
        }

        public Startup ConfigureClient()
        {
            Bot client = new Bot();

            client.InitAsync(AppSettings.Get("bot:token")).Wait();

            return this;
        }

        public async Task InitAsync()
        {
            await Task.Delay(-1);
        }
    }
}
