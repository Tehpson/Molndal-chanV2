namespace mölndal_chan_v2
{
    using Discord;
    using Discord.WebSocket;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class Timed
    {
        public static async Task ClearAllBotChannlesAsync()
        {
            {
                var data = PublicConfig.GetSettings();
                foreach (SocketGuild guild in Program._Client.Guilds)
                {
                    var clear_bot_chat = data.Servers.First(x => x.ServerID == guild.Id).ClearBorChannle;

                    if (clear_bot_chat == true)
                    {
                        var bot_chat_id = data.Servers.First(x => x.ServerID == guild.Id).BotChannel;
                        if (bot_chat_id != null)
                        {
                            var channel = (IMessageChannel)Program._Client.GetChannel((ulong)bot_chat_id);
                            var messages = await channel.GetMessagesAsync(100).FlattenAsync();
                            await (channel as SocketTextChannel).DeleteMessagesAsync(messages);
                            debug.ConsolePrint.Print(guild, "ALL MESSAGE HAVE BEEN DELETET");
                            await channel.SendMessageAsync("all messages have been deleted");
                        }
                    }
                }
            }
        }

        public static async Task SendLEET()
        {
            var data = PublicConfig.GetSettings();
            foreach (var guild in Program._Client.Guilds)
            {
                var bot_chat_id = data.Servers.First(x => x.ServerID == guild.Id).BotChannel;
                if (bot_chat_id != null)
                {
                    var channel = (IMessageChannel)Program._Client.GetChannel((ulong)bot_chat_id);
                    Console.WriteLine($"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}:{DateTime.Now.Second:00}" + " " + channel.Name + "\tLEET ");
                    await channel.SendMessageAsync("LEET");
                }
            }
        }
    }
}