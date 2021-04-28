namespace mölndal_chan_v2.debug
{
    using System;

    public static class ConsolePrint
    {
        public static void Print(Discord.IGuild guild, string command, Discord.WebSocket.SocketUser user = null)
        {
            var time = DateTime.Now.ToString("g");
            var userstr = user == null ? "Server" : user.Username;

            Console.WriteLine($"{time}  -  {guild.Name}  -  {userstr}  -  {command}");
        }
    }
}