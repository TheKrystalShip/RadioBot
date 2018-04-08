using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;

namespace RadioBot.Handlers
{
	public class EventHandler
    {
		private DiscordSocketClient Client;

		public EventHandler(DiscordSocketClient client)
		{
			Client = client;
			Client.Log += Client_Log;
		}

		private async Task Client_Log(LogMessage logMessage)
		{
			if (!logMessage.Message.Contains("OpCode"))
			{
				await Task.Run(() => {
					Console.WriteLine(logMessage);
				});
			}
		}
	}
}
