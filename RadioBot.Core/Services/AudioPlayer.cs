using Discord.Audio;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using TheKrystalShip.Logging;
using TheKrystalShip.RadioBot.Tools;

namespace TheKrystalShip.RadioBot.Core.Services
{
    public class AudioPlayer : IDisposable
    {
        public bool IsRunning { get; private set; } = false;
        public bool IsPlaying { get; private set; } = false;
        public float Volume { get; set; } = 0.25f;
        public Process AudioProcess { get; private set; } = null;
        public Stream DiscordAudioStream { get; private set; } = null;
        private CancellationToken _cancellationToken;

        private readonly ILogger<AudioPlayer> _logger;
        private readonly int _blockSize = 3840;

        public AudioPlayer(CancellationToken cancellationToken)
        {
            _logger = new Logger<AudioPlayer>();
            _logger.LogInformation($"Created {GetType().Name}");

            _cancellationToken = cancellationToken;
        }

        public void ResetCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
        }

        public async Task PlayAsync(IAudioClient audioClient, string searchQuery)
        {
            AudioProcess = CreateAudioProcess(searchQuery);
            DiscordAudioStream = audioClient.CreatePCMStream(AudioApplication.Music);
            IsPlaying = true;

            while (!_cancellationToken.IsCancellationRequested)
            {
                if (AudioProcess is null || AudioProcess.HasExited)
                    break;

                if (DiscordAudioStream is null)
                    break;

                // Pause & Resume commands
                if (!IsPlaying)
                    continue;

                int blockSize = _blockSize;
                byte[] buffer = new byte[blockSize];
                int byteCount;
                byteCount = await AudioProcess.StandardOutput.BaseStream.ReadAsync(buffer.AsMemory(0, blockSize));

                if (byteCount <= 0)
                    break;

                try
                {
                    await DiscordAudioStream.WriteAsync(ScaleVolumeSafeAllocateBuffers(buffer, Volume).AsMemory(0, byteCount));
                }
                catch (Exception e)
                {
                    _logger.LogError(e);
                    break;
                }
            }

            try
            {
                AudioProcess?.Kill();
                AudioProcess?.WaitForExit();
                DiscordAudioStream?.Flush();
                DiscordAudioStream?.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }

            AudioProcess = null;
            DiscordAudioStream = null;
            IsPlaying = false;
        }

        private byte[] ScaleVolumeSafeAllocateBuffers(byte[] audioSamples, float volume)
        {
            if (audioSamples == null)
                return null;

            if (audioSamples.Length % 2 != 0)
                return null;

            if (volume < 0.0f || volume > 1.0f)
                return null;

            byte[] output = new byte[audioSamples.Length];

            try
            {
                if (Math.Abs(volume - 1f) < 0.0001f)
                {
                    Buffer.BlockCopy(audioSamples, 0, output, 0, audioSamples.Length);
                    return output;
                }

                int volumeFixed = (int)Math.Round(volume * 65536d);

                for (var i = 0; i < output.Length; i += 2)
                {
                    int sample = (short)((audioSamples[i + 1] << 8) | audioSamples[i]);
                    int processed = (sample * volumeFixed) >> 16;

                    output[i] = (byte)processed;
                    output[i + 1] = (byte)(processed >> 8);
                }

                return output;
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return null;
            }
        }

        private Process CreateAudioProcess(string query)
        {
            string extension = Configuration.OsIsWindows() ? ".bat" : ".sh";

            return Process.Start(new ProcessStartInfo()
            {
                FileName = string.Concat("init_audio_stream", extension),
                Arguments = query,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
            );
        }

        public void SetVolume(float volume)
        {
            if (volume < 0.0f)
            {
                volume = 0.0f;
            }
            else if (volume > 1.0f)
            {
                volume = 1.0f;
            }

            Volume = volume;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            IsPlaying = true;
        }

        public void Dispose()
        {
            IsPlaying = false;
            IsRunning = false;
            AudioProcess?.Dispose();
            DiscordAudioStream?.Dispose();
        }
    }
}
