using Discord;
using Discord.Audio;
using Discord.Commands;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
			IAudioClient audioClient = await channel.ConnectAsync();

			var guildId = context.Guild.Id;

			if (!AudioClients.ContainsKey(guildId))
			{
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
				}
				catch (Exception)
				{
					Console.WriteLine(new LogMessage(LogSeverity.Info, "RadioService", "Disconnected"));
				}

				AudioClients.TryRemove(context.Guild.Id, out audioClient);
			}
		}

		public async Task PlayAsync(string content, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			if (AudioClients.TryGetValue(guildId, out IAudioClient client))
			{
				// Magic happens here
				using (var ffmpegStream = CreateStream(content).StandardOutput.BaseStream)
				using (var discordOutStream = client.CreatePCMStream(AudioApplication.Music))
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

		private Process CreateStream(string path)
		{
			return Process.Start(new ProcessStartInfo
			{
				FileName = "Programs/ffmpeg.exe",
				Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
		}
	}
}
