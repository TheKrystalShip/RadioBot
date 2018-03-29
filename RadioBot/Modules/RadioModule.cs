using Discord;
using Discord.Commands;

using RadioBot.Services;

using System.Threading.Tasks;

namespace RadioBot.Modules
{
	public class RadioModule : ModuleBase<SocketCommandContext>
    {
		private RadioService RadioService;

		public RadioModule(RadioService radioService) 
			=> RadioService = radioService;

		[Command("join", RunMode = RunMode.Async)]
		public async Task JoinVoiceChannelAsync(IVoiceChannel channel = null)
		{
			var msg = Context.Message;
			channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;

			if (channel is null)
			{
				await ReplyAsync("You must be in a voice channel or either specify a channel to join");
				return; 
			}

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

			var msg = Context.Message;
			IVoiceChannel channel = (msg.Author as IGuildUser)?.VoiceChannel;

			if (channel is null)
			{
				await ReplyAsync("You must be in a voice channel or either specify a channel to join");
				return;
			}

			var audioClient = await channel.ConnectAsync();
			await RadioService.JoinChannel(audioClient, Context);

			await RadioService.PlayAsync(content, Context);
		}
    }
}
