using Discord;
using Discord.Commands;

using RadioBot.Services;

using System.Threading.Tasks;

namespace RadioBot.Modules
{
	[RequireUserPermission(GuildPermission.Administrator)]
	public class RadioModule : ModuleBase<SocketCommandContext>
    {
		private RadioService RadioService;

		// Dependency injection of RadioService
		public RadioModule(RadioService radioService) 
			=> RadioService = radioService;

		[Command("join", RunMode = RunMode.Async)]
		public async Task JoinVoiceChannelAsync(IVoiceChannel channel = null)
		{
			// Am i already in (a || the correct) voice channel?
			var voiceChannel = (Context.Client.CurrentUser as IGuildUser)?.VoiceChannel;

			if (voiceChannel != null || voiceChannel != channel)
			{
				await ReplyAsync("I'm already in a voice channel");
				return;
			}

			// Is user requesting in a voice channel?
			var msg = Context.Message;
			channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;

			if (channel is null)
			{
				await ReplyAsync("You must be in a voice channel or either specify a channel to join");
				return; 
			}

			// Sent request to service
			var audioClient = await channel.ConnectAsync();
			await RadioService.JoinChannel(audioClient, Context);
		}

		[Command("leave", RunMode = RunMode.Async)]
		public async Task LeaveAsync(IVoiceChannel channel = null)
		{
			var msg = Context.Message;
			channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;

			if (channel is null)
			{
				await ReplyAsync("You must be in a voice channel or either specify a channel to join");
				return;
			}

			// Service handles consistency checks on this one
			await RadioService.LeaveChannel(Context);
		}

		[Command("play", RunMode = RunMode.Async)]
		public async Task PlayAsync([Remainder] string content = null)
		{
			if (content is null)
			{
				await ReplyAsync("You had one job...");
				return;
			}

			// Is user requesting in a voice channel?
			var msg = Context.Message;
			IVoiceChannel channel = (msg.Author as IGuildUser)?.VoiceChannel;

			if (channel is null)
			{
				await ReplyAsync("You must be in a voice channel or either specify a channel to join");
				return;
			}

			// Am i already in a voice channel?
			var voiceChannel = (Context.Client.CurrentUser as IGuildUser)?.VoiceChannel;

			if (voiceChannel is null)
			{
				var audioClient = await channel.ConnectAsync();
				await RadioService.JoinChannel(audioClient, Context);
			}

			// Send play request to service
			await RadioService.PlayAsync(content, Context);
		}
    }
}
