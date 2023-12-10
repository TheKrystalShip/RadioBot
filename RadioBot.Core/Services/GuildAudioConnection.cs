using System;
using System.Threading;
using Discord.Audio;

namespace TheKrystalShip.RadioBot.Core.Services
{
    public class GuildAudioConnection : IDisposable
    {
        public IAudioClient DiscordAudioClient { get; set; }
        public AudioPlayer AudioPlayer { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }

        public GuildAudioConnection(IAudioClient client)
        {
            DiscordAudioClient = client;
            CancellationTokenSource = new CancellationTokenSource();
            AudioPlayer = new AudioPlayer(CancellationTokenSource.Token);
        }

        public GuildAudioConnection() : this(null)
        {

        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel(true);
            CancellationTokenSource.Dispose();
            // DiscordAudioClient.Dispose();
            AudioPlayer.Dispose();
        }
    }
}
