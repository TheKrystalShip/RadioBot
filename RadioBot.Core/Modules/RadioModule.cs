using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot.Core.Modules
{
    public class RadioModule : Module
    {
        [Command("join")]
        public async Task JoinVoiceChannelAsync()
        {
            IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if (userVoiceChannel is null)
            {
                await ReplyAsync("You're not in a voice channel");
                return;
            }

            await RadioService.JoinChannelAsync(userVoiceChannel);
        }

        [Command("leave")]
        [Alias("stop", "fuck off")]
        public async Task LeaveAsync()
        {
            await RadioService.LeaveChannelAsync();
        }

        [Command("play")]
        public async Task PlayAsync([Remainder] string content = null)
        {
            if (content is null)
            {
                await ReplyAsync("You need to tell me something to play");
                return;
            }

            IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if (userVoiceChannel is null)
            {
                await ReplyAsync("You're not in a voice channel");
                return;
            }

            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("\u25B6"));

            await RadioService.JoinChannelAsync(userVoiceChannel);
            await RadioService.PlayAsync(content);
        }

        [Command("pause")]
        public async Task PausePlaybackAsync()
        {
            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("\u23F8"));

            RadioService.Pause();
        }

        [Command("resume")]
        [Alias("continue")]
        public async Task ResumePlaybackAsync()
        {
            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("\u25B6"));

            RadioService.Resume();
        }

        [Command("volume")]
        public async Task SetVolumeAsync(float volume)
        {
            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("👍🏻"));

            RadioService.SetVolume(volume / 100);
        }
    }
}
