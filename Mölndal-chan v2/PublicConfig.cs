namespace mölndal_chan_v2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Discord.WebSocket;
    using Discord;
    using Newtonsoft.Json;

    internal static class PublicConfig
    {
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mölndal-Chan");

        /// <summary>
        /// configure the setting
        /// </summary>
        /// <param name="setting">what setting to change</param>
        /// <param name="value">change to:</param>
        /// <param name="id">id of the server</param>
        public static void ConfigData(string setting, object value, ulong id)
        {
            var data = GetSettings();
            var server = data.Servers.First(s => s.ServerID == id);
            switch (setting)
            {
                case "prefix":
                    try { server.Prefix = (string)value; } catch (Exception ex) { return; }
                    break;
                case "role.add":
                    try {
                        server.RolesID.Add((ulong)value); } catch (Exception ex) { return; }
                    break;
                case "role.remove":
                    try { server.RolesID.Remove((ulong)value); } catch (Exception ex) { return; }
                    break;
                case "botChannel.remove":
                    try{server.BotChannel = null;} catch(Exception ex) { return; }
                    break;
                case "botChannel.add":
                    try { server.BotChannel = (ulong)value; } catch (Exception ex) { return; }
                    break;
                case "clearBotChannle":
                    try { server.ClearBorChannle = (bool)value; } catch(Exception ex) { return; }
                    break;

                default:
                    debug.ConsolePrint.Print(null, "Config data didn't exist");
                    return;
            }
            var jsonInput = JsonConvert.SerializeObject(data);
            File.WriteAllText(Path, jsonInput);
        }

        /// <summary>
        /// get settings from json file
        /// </summary>
        public static Root GetSettings() => JsonConvert.DeserializeObject<Root>(File.ReadAllText(Path));

        /// <summary>
        /// Create the Setting file
        /// </summary>
        public static void Initilaze()
        {
            Directory.CreateDirectory(Path);
            Path = System.IO.Path.Combine(Path, "ServerData.json");
            if (!File.Exists(Path))
            {
                var root = new Root();
                foreach (SocketGuild guild in Program._Client.Guilds)
                {
                    root.Servers.Add(new Server { ServerID = guild.Id ,ServerName = guild.Name});
                }
                var jsonInput = JsonConvert.SerializeObject(root, Formatting.Indented);
                File.WriteAllText(Path, jsonInput);
            }
        }
    }


    public class Server
    {
        public ulong ServerID { get; set; }
        public string ServerName { get; set; }
        public string Prefix { get; set; } = "!";
        public ulong? BotChannel { get; set; } = null;
        public bool ClearBorChannle { get; set; } = false;
        public List<ulong> RolesID { get; set; } = new List<ulong>();

        public override string ToString()
        {
            return $"{ServerName}({ServerID})";
        }
    }

    public class Root
    {
        public List<Server> Servers { get; set; } = new List<Server>();
    }
}
