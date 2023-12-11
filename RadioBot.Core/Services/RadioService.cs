using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Core
{
    public class RadioService
    {
        private SocketSlashCommand _commandContext;

        private readonly ConcurrentDictionary<ulong, GuildAudioConnection> _guildAudioConnections;
        private readonly Logger<RadioService> _logger;

        public RadioService()
        {
            _guildAudioConnections = new ConcurrentDictionary<ulong, GuildAudioConnection>();

            _logger = new Logger<RadioService>();
            _logger.LogInformation($"Started {GetType().Name}");
        }

        public void SetContext(SocketSlashCommand command)
        {
            _commandContext = command;
        }

        public async Task HandleJoinCommandAsync()
        {
            IVoiceChannel userVoiceChannel = (_commandContext.User as IGuildUser)?.VoiceChannel;

            if (userVoiceChannel is null)
            {
                await _commandContext.RespondAsync("You're not in a voice channel");
                return;
            }

            ulong guildId = (ulong) _commandContext.GuildId;

            if (!_guildAudioConnections.ContainsKey(guildId))
            {
                IAudioClient audioClient = await userVoiceChannel.ConnectAsync();
                GuildAudioConnection guildAudioConnection = new GuildAudioConnection(audioClient);

                _guildAudioConnections.TryAdd(guildId, guildAudioConnection);

                _logger.LogInformation($"Connected to {userVoiceChannel.Name}");
            }
            else
            {
                _logger.LogError($"_guildAudioConnections already contains a key entry for guild {guildId}");
            }
        }

        public async Task HandlePlayCommandAsync()
        {
            ulong guildId = (ulong) _commandContext.GuildId;
            string content = _commandContext.Data.Options.First().ToString() ?? string.Empty;

            if (content == string.Empty)
            {
                await _commandContext.RespondAsync("Please specify a song name");
                return;
            }

            if (!_guildAudioConnections.TryGetValue(guildId, out GuildAudioConnection guildAudioConnection))
            {
                _logger.LogError("Failed to retrieve audio client, maybe not in voice channel");

                _logger.LogInformation("Trying to join voice channel...");
                await HandleJoinCommandAsync();

                // Try again to get the audio client after joining
                if (!_guildAudioConnections.TryGetValue(guildId, out guildAudioConnection))
                {
                    _logger.LogError("Failed to retrieve audio client, maybe not in voice channel");
                    await _commandContext.RespondAsync("Failed to retrieve audio client");
                    return;
                }
            }

            // If there's something playing, we gotta stop the existing AudioPlayer and create a new one
            if (guildAudioConnection.AudioPlayer.IsPlaying)
            {
                // Save the DiscordAudioClient instance from being disposed
                IAudioClient audioClient = guildAudioConnection.DiscordAudioClient;

                // Stop & Dispose of existing AudioPlayer
                await HandleStopCommandAsync();

                // Create a new AudioPlayer but keep existing DiscordAudioClient
                if (!_guildAudioConnections.TryAdd(guildId, new GuildAudioConnection(audioClient)))
                {
                    _logger.LogError("Failed to add new AudioClient");
                }
            }

            await guildAudioConnection.AudioPlayer.PlayAsync(guildAudioConnection.DiscordAudioClient, content);

            await _commandContext.RespondAsync("Finished queue");

            if (_guildAudioConnections.TryRemove(guildId, out guildAudioConnection))
            {
                await guildAudioConnection.DiscordAudioClient.StopAsync();
                guildAudioConnection.Dispose();
            }
        }

        public async Task HandlePauseCommandAsync()
        {
            if (_guildAudioConnections.TryGetValue((ulong)_commandContext.GuildId, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.AudioPlayer.Pause();
            }
        }

        public async Task HandleResumeCommandAsync()
        {
            if (_guildAudioConnections.TryGetValue((ulong)_commandContext.GuildId, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.AudioPlayer.Resume();
            }
        }

        public async Task HandleStopCommandAsync()
        {
            if (_guildAudioConnections.TryRemove((ulong)_commandContext.GuildId, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.Dispose();
            }
        }

        public async Task HandleVolumeCommandAsync()
        {
            float volume = AudioPlayer.DEFAULT_VOLUME;

            try
            {
                volume = float.Parse(_commandContext.Data.Options.First().ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await _commandContext.RespondAsync("Failed to parse new volume, reverting to default");
            }

            if (_guildAudioConnections.TryGetValue((ulong)_commandContext.GuildId, out GuildAudioConnection guildAudioConnection))
            {
                guildAudioConnection.AudioPlayer.SetVolume(volume);
            }
        }

        public async Task HandleLeaveCommandAsync()
        {
            ulong guildId = (ulong)_commandContext.GuildId;

            if (!_guildAudioConnections.TryGetValue(guildId, out GuildAudioConnection guildAudioConnection))
            {
                await _commandContext.Channel.SendMessageAsync("I'm not connected ya dumbfuck");
            }

            _guildAudioConnections.TryRemove(new KeyValuePair<ulong, GuildAudioConnection>(guildId, guildAudioConnection));

            await guildAudioConnection.DiscordAudioClient?.StopAsync();
            guildAudioConnection.Dispose();

            await _commandContext.Channel.SendMessageAsync("Disconnected from voice channel");
        }
    }
}
