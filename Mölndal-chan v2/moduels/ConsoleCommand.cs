namespace Mölndal_chan_v2.moduels
{
    using mölndal_chan_v2;
    using System;
    using System.Collections.Generic;
    using System.Text;

    class ConsoleCommand
    {
        internal static void ConsoleCommandDisp(string message)
        {
            switch (message.ToLower())
            {
                case "status":
                    Status();
                    break;
                default:
                    Console.WriteLine("no command found");
                    break;
            }
        }
        private static void Status()
        {
            var ts = Program.stop_watch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00} ";
            Console.WriteLine("Status: Online " +
                "\nRunTime: " + elapsedTime);
        }
    }
}
