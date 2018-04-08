using Discord;
using Discord.Audio;
using Discord.Commands;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RadioBot.Services
{
	public class RadioService
    {
		// Stores each guild's audio client
		private readonly ConcurrentDictionary<ulong, IAudioClient> AudioClients;

		public RadioService()
		{
			AudioClients = new ConcurrentDictionary<ulong, IAudioClient>();
		}

		public async Task JoinChannelAsync(IVoiceChannel channel, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;

			if (!AudioClients.ContainsKey(guildId))
			{
				IAudioClient audioClient = await channel.ConnectAsync();
				AudioClients.TryAdd(guildId, audioClient);
			}
		}

		public async Task LeaveChannel(SocketCommandContext context)
		{
			var guildId = context.Guild.Id;

			if (AudioClients.TryGetValue(guildId, out IAudioClient audioClient))
			{
				try
				{
					await audioClient.StopAsync();
					audioClient.Dispose();
				}
				catch (Exception)
				{
					Console.WriteLine(new LogMessage(LogSeverity.Info, "RadioService", "Disconnected"));
				}

				AudioClients.TryRemove(context.Guild.Id, out audioClient);
			}
			else
			{
				await context.Channel.SendMessageAsync("I'm not connected");
			}
		}

		public async Task PlayAsync(string content, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			if (AudioClients.TryGetValue(guildId, out IAudioClient client))
			{
				// Magic happens here
				using (Stream ffmpegStream = CreateStream(content))
				using (AudioOutStream discordOutStream = client.CreatePCMStream(AudioApplication.Music))
				{
					try
					{
						await ffmpegStream.CopyToAsync(discordOutStream);
					}
					catch (Exception)
					{
						Console.WriteLine(new LogMessage(LogSeverity.Error, "RadioService", "Closed audio stream"));
					}
					finally
					{
						await discordOutStream.FlushAsync();
					}
				}
			}
			else
			{
				Console.WriteLine(new LogMessage(LogSeverity.Error, "RadioService", "Failed to retrieve AudioClient"));
			}
		}

		private Stream CreateStream(string path)
		{
			return Process.Start(new ProcessStartInfo()
				{
					FileName = "ffmpeg.exe",
					Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			).StandardOutput.BaseStream;
		}
	}
}
