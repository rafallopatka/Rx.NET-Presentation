using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_Interval
{
    class Program
    {
        static void Main(string[] args)
        {
            //IObservable<long> timeStream = Observable.Interval(TimeSpan.FromSeconds(1));

            //timeStream
            //    .Select(signal => new { IsNewContentAvalaible = CheckRemoteContent() })
            //    .Where(signal => signal.IsNewContentAvalaible)
            //    .Select(signal => DownloadNewContent())
            //    .Subscribe(newContent =>
            //    {
            //        Console.WriteLine($"New content: {newContent}");
            //    });

            IObservable<long> timeStream = Observable.Interval(TimeSpan.FromSeconds(1));
                timeStream
                .Timestamp()
                .Do(signal => Log($"\n[{signal.Value}][{signal.Timestamp}]:\nChecking remote content"))
                .Select(signal => new
                {
                    IsNewContentAvalaible = CheckRemoteContent()
                })
                .Do(signal => Log($"\t|Is new content avalaible: {signal.IsNewContentAvalaible}",
                                    signal.IsNewContentAvalaible ? ConsoleColor.Green : ConsoleColor.Red))
                .Where(signal => signal.IsNewContentAvalaible)
                .Do(signal => Log($"\t\t|Downloading new content", ConsoleColor.Blue))
                .Select(signal => DownloadNewContent())
                .Subscribe(newContent =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\t\t\t|New content: {newContent}");
                    Console.ResetColor();
                });



            Console.ReadLine();
        }

        private static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static string DownloadNewContent()
        {
            return Guid.NewGuid().ToString();
        }

        private static int _index = 0;

        private static bool CheckRemoteContent()
        {
            return _index++ % 5 == 0;
        }
    }
}
