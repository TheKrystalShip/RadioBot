using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TheKrystalShip.DependencyInjection;
using TheKrystalShip.Logging;
using TheKrystalShip.RadioBot.Tools;

namespace TheKrystalShip.RadioBot.Core
{
    public class CommandHandler : CommandService
    {
        private readonly ILogger<CommandHandler> _logger;
        private DiscordSocketClient _client;
        private RadioService _radioService;

        private static readonly CommandServiceConfig __defaultCommandServiceConfig = new()
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
            DefaultRunMode = RunMode.Async
        };

        public CommandHandler() : this(__defaultCommandServiceConfig)
        {

        }

        public CommandHandler(CommandServiceConfig config) : base(config)
        {
            _logger = new Logger<CommandHandler>();
            _radioService = Container.Get<RadioService>();

            Log += OnLogAsync;
        }

        public void SetClient(DiscordSocketClient client)
        {
            _client = client;
            _client.Log += OnLogAsync;
            _client.SlashCommandExecuted += HandleCommand;

            // Only has to run once unless commands are changed. Keep commented
            //_client.Ready += RegisterSlashCommands;
        }

        public async Task HandleCommand(SocketSlashCommand command)
        {
            // All commands require the user to specify a game.
            // string game = (string)command.Data.Options.First().Value;
            _radioService.SetContext(command);

            switch (command.Data.Name)
            {
                case "join":
                    await _radioService.HandleJoinCommandAsync();
                    break;
                case "play":
                    await _radioService.HandlePlayCommandAsync();
                    break;
                case "pause":
                    await _radioService.HandlePauseCommandAsync();
                    break;
                case "resume":
                    await _radioService.HandleResumeCommandAsync();
                    break;
                case "stop":
                    await _radioService.HandleStopCommandAsync();
                    break;
                case "volume":
                    await _radioService.HandleVolumeCommandAsync();
                    break;
                case "leave":
                    await _radioService.HandleLeaveCommandAsync();
                    break;
            }
        }

        public Task OnLogAsync(LogMessage message)
        {
            if (message.Exception is null)
            {
                _logger.LogInformation(GetType().Name + $" ({message.Source})", message.Message);
            }
            else
            {
                if (message.Exception.InnerException != null)
                {
                    _logger.LogError(message.Exception.InnerException);
                }

                _logger.LogError(message.Exception);
            }
            return Task.CompletedTask;
        }

        // Only needs to be called once
        public async Task RegisterSlashCommands()
        {
            // Commands are built for a specific guild, global commands require a lot higher
            // level of permissions and they are not needed for our use case.
            string guildId = AppSettings.Get("bot:guildId") ?? "";
            SocketGuild guild = _client.GetGuild(ulong.Parse(guildId));

            List<ApplicationCommandProperties> commandsToRegister = [
                new SlashCommandBuilder()
                    .WithName("join")
                    .WithDescription("Join the voice channel")
                    .Build(),
                new SlashCommandBuilder()
                    .WithName("play")
                    .WithDescription("Play a specific song")
                    .AddOption("song", ApplicationCommandOptionType.String, "Song name or YouTube URL", isRequired: true)
                    .Build(),
                new SlashCommandBuilder()
                    .WithName("pause")
                    .WithDescription("Pause currently playing song")
                    .Build(),
                new SlashCommandBuilder()
                    .WithName("resume")
                    .WithDescription("Continue playing paused song")
                    .Build(),
                new SlashCommandBuilder()
                    .WithName("stop")
                    .WithDescription("Stop audio playback")
                    .Build(),
                new SlashCommandBuilder()
                    .WithName("volume")
                    .WithDescription("Set the playback volume")
                    .AddOption("value", ApplicationCommandOptionType.Integer, "Value between 0 and 100", isRequired: true)
                    .Build(),
                new SlashCommandBuilder()
                    .WithName("leave")
                    .WithDescription("Stop audio playback and leave voice channel")
                    .Build()
            ];

            try
            {
                // This takes time and will block the gateway
                foreach (var command in commandsToRegister)
                {
                    await guild.CreateApplicationCommandAsync(command);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
        }
    }
}
