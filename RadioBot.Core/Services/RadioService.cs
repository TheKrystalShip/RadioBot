using Discord;
using Discord.Audio;
using Discord.Commands;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Core.Services
{
    public class RadioService : IRadioService
    {
        private SocketCommandContext _context;

        private readonly ConcurrentDictionary<ulong, GuildAudioConnection> _guildAudioConnections;
        private readonly ILogger<RadioService> _logger;

        public RadioService()
        {
            _guildAudioConnections = new ConcurrentDictionary<ulong, GuildAudioConnection>();

            _logger = new Logger<RadioService>();
            _logger.LogInformation($"Started {GetType().Name}");
        }

        public void SetContext(SocketCommandContext context)
        {
            _context = context;
        }

        public async Task JoinChannelAsync(IVoiceChannel channel)
        {
            ulong guildId = _context.Guild.Id;

            if (!_guildAudioConnections.ContainsKey(guildId))
            {
                IAudioClient audioClient = await channel.ConnectAsync();
                GuildAudioConnection guildAudioConnection = new GuildAudioConnection(audioClient);

                _guildAudioConnections.TryAdd(guildId, guildAudioConnection);

                _logger.LogInformation($"Connected to {channel.Name}");
            }
        }

        public async Task LeaveChannelAsync()
        {
            ulong guildId = _context.Guild.Id;

            if (!_guildAudioConnections.TryGetValue(guildId, out GuildAudioConnection guildAudioConnection))
            {
                await _context.Channel.SendMessageAsync("I'm not connected ya dumbfuck");
            }

            _guildAudioConnections.TryRemove(new KeyValuePair<ulong, GuildAudioConnection>(guildId, guildAudioConnection));

            await guildAudioConnection.DiscordAudioClient?.StopAsync();
            guildAudioConnection.Dispose();

            await _context.Channel.SendMessageAsync("Disconnected from voice channel");
        }

        public async Task PlayAsync(string content)
        {
            ulong guildId = _context.Guild.Id;

            if (!_guildAudioConnections.TryGetValue(guildId, out GuildAudioConnection guildAudioConnection))
            {
                _logger.LogError("Failed to retrieve audio client");
                return;
            }

            if (guildAudioConnection.AudioPlayer.IsPlaying)
            {
                Stop();
            }

            await guildAudioConnection.AudioPlayer.PlayAsync(guildAudioConnection.DiscordAudioClient, content);

            await _context.Channel.SendMessageAsync("Finished queue");

            if (_guildAudioConnections.TryRemove(guildId, out guildAudioConnection))
            {
                await guildAudioConnection.DiscordAudioClient.StopAsync();
                guildAudioConnection.Dispose();
            }
        }

        public void Pause()
        {
            if (_guildAudioConnections.TryGetValue(_context.Guild.Id, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.AudioPlayer.Pause();
            }
        }

        public void Resume()
        {
            if (_guildAudioConnections.TryGetValue(_context.Guild.Id, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.AudioPlayer.Resume();
            }
        }

        public void SetVolume(float volume)
        {
            if (_guildAudioConnections.TryGetValue(_context.Guild.Id, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.AudioPlayer.SetVolume(volume);
            }
        }

        public void Stop()
        {
            if (_guildAudioConnections.TryRemove(_context.Guild.Id, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.Dispose();
            }
        }
    }
}
