using Discord.Commands;

using TheKrystalShip.DependencyInjection;
using TheKrystalShip.RadioBot.Core.Services;

namespace TheKrystalShip.RadioBot.Core.Modules
{
    public class Module : ModuleBase<SocketCommandContext>
    {
        public IRadioService RadioService { get; private set; }

        public Module()
        {
            RadioService = Container.Get<IRadioService>();
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);

            RadioService.SetContext(Context);
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);

        }
    }
}
