# RadioBot

[![Build status](https://ci.appveyor.com/api/projects/status/ornlq9rscs71pw8t?svg=true)](https://ci.appveyor.com/project/Flysenberg/radiobot)

Discord music bot written in C# using the Discord.Net Api Wrapper (1.0.2) made by @RogueException.

## Audio playback libraries & programs

These binaries & libraries are in the \RadioBot folder already.

Audio playback requires two libraries, `libsodium.dll` and `opus.dll`, found in the official documentation [here](https://discord.foxbot.me/docs/guides/voice/sending-voice.html#installing).

Or [here](https://dsharpplus.github.io/natives/index.html) if the download link is dead on the official website.

> On Windows, rename `libopus.dll` to `opus.dll`.

Audio downloading & encoding are handled by [yt-dlp](https://github.com/yt-dlp/yt-dlp) and [ffmpeg](https://www.ffmpeg.org/).
