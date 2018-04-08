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
		public async Task JoinVoiceChannelAsync(IVoiceChannel channel = null)
		{
			// Am i already in (a || the correct) voice channel?
			IVoiceChannel myVoiceChannel = (Context.Client.CurrentUser as IGuildUser)?.VoiceChannel;
			IVoiceChannel userVoiceChannel = channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;

			if (myVoiceChannel == userVoiceChannel)
			{
				await ReplyAsync("I'm already in a voice channel");
				return;
			}

			// Sent request to service
			await RadioService.JoinChannelAsync(userVoiceChannel, Context);
		}

		[Command("leave")]
		[Alias("stop")]
		public async Task LeaveAsync()
		{
			IVoiceChannel myVoiceChannel = (Context.Client.CurrentUser as IGuildUser).VoiceChannel;

			if (myVoiceChannel is null)
			{
				await ReplyAsync("I'm not connected");
				return;
			}

			// Service handles checks on this one
			await RadioService.LeaveChannel(Context);
		}

		[Command("play")]
		public async Task PlayAsync([Remainder] string content = null)
		{
			if (content is null)
			{
				await ReplyAsync("You had one job...");
				return;
			}

			// Is user requesting in a voice channel?
			IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

			if (userVoiceChannel is null)
			{
				await ReplyAsync("You must be in a voice channel");
				return;
			}

			// Am i already in a voice channel, or in the same channel as the user?
			IVoiceChannel myVoiceChannel = (Context.Client.CurrentUser as IGuildUser)?.VoiceChannel;

			if (myVoiceChannel is null | myVoiceChannel != userVoiceChannel)
			{
				await RadioService.JoinChannelAsync(userVoiceChannel, Context);
			}

			// Change status to "Playing {content}" whatever the user requested
			await Context.Client.SetGameAsync(content);

			// Send play request to service
			await RadioService.PlayAsync(content, Context);
		}
    }
}
