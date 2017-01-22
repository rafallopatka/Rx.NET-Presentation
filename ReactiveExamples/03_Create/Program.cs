using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Timers;

// ReSharper disable ConvertClosureToMethodGroup

namespace _03_Create
{
    class Program
    {
        static void Main(string[] args)
        {

             Observable.Create<long>(observer =>
                {
                    int index = 0;
                    var timer = new Timer {Interval = 1000};
                    ElapsedEventHandler handler = (s, e) => observer.OnNext(index++);
                    timer.Elapsed += handler;
                    timer.Start();
                    return Disposable.Create(() =>
                    {
                        timer.Elapsed -= handler;
                        timer.Dispose();
                    });
                })
                .Subscribe(index => Console.WriteLine(index));

            Console.ReadLine();
        }
    }
}
