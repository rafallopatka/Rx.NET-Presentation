using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace _09_ObserveOn
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"[Main ThreadId] {Thread.CurrentThread.ManagedThreadId}");
            IObservable<int> observable = Observable.Create<int>(observer =>
            {
                Console.WriteLine($"[Start ThreadId] {Thread.CurrentThread.ManagedThreadId}");
                for (int i = 0; i < 3; i++)
                    observer.OnNext(i);

                observer.OnCompleted();
                return Disposable.Empty;
            });
            observable
                .Take(1)
                .SubscribeOn(new NewThreadScheduler())
                .Do(x => Console.WriteLine($"[After SubscribeOn ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
                .ObserveOn(new NewThreadScheduler())
                .Do(x => Console.WriteLine($"[After ObserveOn #1 ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
                .ObserveOn(new NewThreadScheduler())
                .Do(x => Console.WriteLine($"[After ObserveOn #2 ThreadId] {Thread.CurrentThread.ManagedThreadId}"))
                .Subscribe(x => Console.WriteLine($"[Subscribe ThreadId] {Thread.CurrentThread.ManagedThreadId}"));

            Console.ReadLine();
        }
    }
}
