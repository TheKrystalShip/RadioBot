using Discord;
using Discord.Commands;

using RadioBot.Services;

using System.Threading.Tasks;

namespace RadioBot.Modules
{
	public class RadioModule : ModuleBase<SocketCommandContext>
    {
		private RadioService RadioService;

		// Dependency injection of RadioService
		public RadioModule(RadioService radioService)
		{
			RadioService = radioService;
		}

		[Command("join")]
		public async Task JoinVoiceChannelAsync()
		{
			// Am i already in (a || the correct) voice channel?
			IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

			if (userVoiceChannel is null)
			{
				await ReplyAsync("You're not in a voice channel");
				return;
			}

			// Sent request to service
			await RadioService.JoinChannelAsync(userVoiceChannel, Context);
		}

		[Command("leave")]
		[Alias("stop")]
		public async Task LeaveAsync()
		{
			// Service handles checks on this one
			await RadioService.LeaveChannel(Context);
		}

		[Command("play")]
		public async Task PlayAsync([Remainder] string content = null)
		{
			// Is user requesting in a voice channel?
			IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

			if (userVoiceChannel is null)
			{
				await ReplyAsync("You're not in a voice channel");
				return;
			}

			await RadioService.JoinChannelAsync(userVoiceChannel, Context);

			// Change status to "Playing {content}" whatever the user requested
			await Context.Client.SetGameAsync(content);

			// Send play request to service
			await RadioService.PlayAsync(content, Context);
		}
    }
}
