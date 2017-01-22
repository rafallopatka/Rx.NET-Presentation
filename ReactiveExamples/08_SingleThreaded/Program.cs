using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;

namespace _08_SingleThreaded
{
    class Program
    {
        static void Main(string[] args)
        {
            MainThreadExample();
            Console.ReadLine();
            SeparateThreadExample();
            Console.ReadLine();
            IntervalExample();
            Console.ReadLine();
        }

        private static void MainThreadExample()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("Main thread example");
            Console.WriteLine("==========================================================");

            Console.WriteLine($"[Main ThreadId] {Thread.CurrentThread.ManagedThreadId}");
            IObservable<int> observable = Observable.Create<int>(observer =>
            {
                for (int i = 0; i < 3; i++)
                    observer.OnNext(i);

                observer.OnCompleted();
                return Disposable.Empty;
            });
            IDisposable subscription = observable
                .Do(x => Console.WriteLine($"[Do ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
                .Subscribe(x => Console.WriteLine($"[Subscribe ThreadId] {Thread.CurrentThread.ManagedThreadId}"));
        }

        private static void SeparateThreadExample()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("Separate thread example");
            Console.WriteLine("==========================================================");

            Console.WriteLine($"[Main ThreadId] {Thread.CurrentThread.ManagedThreadId}");
            IObservable<int> observable = Observable.Create<int>(observer =>
            {
                Task.Run(() =>
                {
                    for (int i = 0; i < 3; i++)
                        observer.OnNext(i);

                    observer.OnCompleted();
                });
                return Disposable.Empty;
            });

            IDisposable subscription = observable
                .Do(x => Console.WriteLine($"[Do ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
                .Subscribe(x => Console.WriteLine($"[Subscribe ThreadId] {Thread.CurrentThread.ManagedThreadId}"));
        }

        private static void IntervalExample()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("Interval example");
            Console.WriteLine("==========================================================");

            Console.WriteLine($"[Main ThreadId] {Thread.CurrentThread.ManagedThreadId}");
            Observable.Interval(TimeSpan.FromSeconds(1))
                .Do(x => Console.WriteLine($"[Do ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
                .Take(5)
                .Subscribe(x => Console.WriteLine($"[Subscribe ThreadId] {Thread.CurrentThread.ManagedThreadId}"));

            //Observable.Create<int>(observer =>
            //{
            //    int i = 0;
            //    var timer = new Timer(state => observer.OnNext(i++));
            //    timer.Change(0, 1000);
            //    return timer;
            //})
            //.Do(x => Console.WriteLine($"[Do ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
            //.Take(10)
            //.Subscribe(x => Console.WriteLine($"[Subscribe ThreadId] {Thread.CurrentThread.ManagedThreadId}"));
        }
    }
}
