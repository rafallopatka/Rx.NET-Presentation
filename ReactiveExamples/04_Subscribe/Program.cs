using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace _04_Subscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            var observable = Observable.Create<int>(observer =>
            {
                for (int i = 0; i < 5; i++)
                    observer.OnNext(i);

                observer.OnCompleted();
                return Disposable.Empty;
            });
            observable.Subscribe(onNext: item =>
            {
                Console.WriteLine($"OnNext: {item}");
            }, onError: exception =>
            {
                Console.WriteLine($"OnError: {exception}");
            }, onCompleted: () =>
            {
                Console.WriteLine("Completed");
            });
        }
    }
}
