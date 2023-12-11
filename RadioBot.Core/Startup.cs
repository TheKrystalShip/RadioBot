using TheKrystalShip.RadioBot.Tools;
using TheKrystalShip.DependencyInjection;
using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot.Core
{
    public class Startup
    {
        public Startup ConfigureCommands()
        {
            Container.Add<CommandHandler>();

            return this;
        }

        public Startup ConfigureServices()
        {
            Container.Add<RadioService>();

            return this;
        }

        public Startup ConfigureClient()
        {
            Bot client = new Bot();
            client.InitAsync(AppSettings.Get("bot:token")).Wait();

            Container.Add(client);

            return this;
        }

        public async Task InitAsync()
        {
            // Call to instantiate
            Container.Get<CommandHandler>();

            await Task.Delay(-1);
        }
    }
}
