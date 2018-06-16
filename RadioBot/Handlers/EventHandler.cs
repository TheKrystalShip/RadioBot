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

		private Task Client_Log(LogMessage logMessage)
		{
			if (!logMessage.Message.Contains("OpCode"))
			{
				Console.WriteLine(logMessage);
			}

			return Task.CompletedTask;
		}
	}
}
