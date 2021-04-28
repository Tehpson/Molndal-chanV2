namespace mölndal_chan_v2
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    internal static class Program
    {
        public static string[] hello_saying, motivation_mental_text;
        public static Stopwatch stop_watch = new Stopwatch();

        public static DiscordSocketClient _Client;
        private static CommandService _Commands;
        private static IServiceProvider _Service;

        public static void Main() => MainAsync().GetAwaiter().GetResult();

        private async static Task MainAsync()
        {
            _Client = new DiscordSocketClient(new DiscordSocketConfig { AlwaysDownloadUsers = true, LogLevel = Discord.LogSeverity.Verbose });
            _Commands = new CommandService(new CommandServiceConfig { LogLevel = Discord.LogSeverity.Verbose, DefaultRunMode = RunMode.Async, CaseSensitiveCommands = false, IgnoreExtraArgs = true });
            _Service = new ServiceCollection()
                .AddSingleton(_Client)
                .AddSingleton(_Commands)
                .BuildServiceProvider();

            _Client.Log += LogAsync;
            _Client.MessageReceived += MessageReceivedAsync;

            const string token = Config.DiscordKEY;

            await _Client.LoginAsync(TokenType.Bot, token);
            await _Client.StartAsync();
            _Client.Ready += OnClientReadyAsync;

            //preapre the time for sending leet
            using (var leetTimer = new System.Timers.Timer(86400000))
            {
                leetTimer.Elapsed += async (sender, e) => await Timed.SendLEET();
                leetTimer.AutoReset = true;

                //time until start leettimer
                var leetTime = (DateTime.Parse("13:37:42") - DateTime.Now).TotalMilliseconds;
                if (leetTime < 0) leetTime += 86400000;
                using (var waitleetTimer = new System.Timers.Timer(leetTime))
                {
                    waitleetTimer.Elapsed += (sender, e) => leetTimer.Start();
                    waitleetTimer.Elapsed += async (sender, e) => await Timed.SendLEET();
                    waitleetTimer.Start();
                    waitleetTimer.AutoReset = false;
                }
            }
            //Clears bot chat one time every hour

            using (var timer = new System.Timers.Timer(60000 * 60))
            {
                timer.Elapsed += async (sedner, e) => await Timed.ClearAllBotChannlesAsync();
                timer.Start();
                timer.AutoReset = true;

                await Task.Delay(-1);
            }
        }

        private async static Task MessageReceivedAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_Client, message);
            if (message.Author.IsBot) return;

            var pos = 0;
            var prefix = PublicConfig.GetSettings().Servers.First(s => s.ServerID == context.Guild.Id).Prefix;
            if (message.HasStringPrefix(prefix, ref pos))
            {
                var result = await _Commands.ExecuteAsync(context, pos, _Service);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _Service);
        }

        private static Task OnClientReadyAsync()
        {
            Setup();
            PublicConfig.Initilaze();
            return Task.CompletedTask;
        }

        private static Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private static void Setup()
        {
            string workingDirectory, path, textDirectory;
            workingDirectory = Environment.CurrentDirectory;
            textDirectory = Path.Combine(workingDirectory, "text");
            try
            {
                //setup hello file
                path = Path.Combine(textDirectory, "hi.txt");
                hello_saying = File.ReadAllLines(path).ToArray();

                //setup mental motivation file
                path = Path.Combine(textDirectory, "motivation_mental.txt");
                motivation_mental_text = File.ReadAllLines(path).ToArray();
            }
            catch (Exception)
            {
                var botDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
                botDirectory = Path.Combine(botDirectory, "Molndal-chan v2");
                textDirectory = Path.Combine(botDirectory, "Text");
                Console.WriteLine(textDirectory);

                //setup hello file
                path = Path.Combine(textDirectory, "hi.txt");
                hello_saying = File.ReadAllLines(path).ToArray();

                //setup mental motivation file
                path = Path.Combine(textDirectory, "motivation_mental.txt");
                motivation_mental_text = File.ReadAllLines(path).ToArray();
            }
            stop_watch.Start();
        }
    }
}