using System;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace _07_Unsubscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            IObservable<long> observable = Observable
                .Interval(TimeSpan.FromSeconds(1));

            IDisposable subscription = observable
                .Subscribe(onNext: index => Console.WriteLine($"OnNext {index}"),
                    onError: exception => Console.WriteLine("OnError"),
                    onCompleted: () => Console.WriteLine("Completed"));

            WaitTwoSeconds();
            subscription.Dispose();
            Console.WriteLine("Subscription canceled");

            Console.ReadLine();
        }

        private static void WaitTwoSeconds()
        {
            Task.Delay(2000).Wait();
        }
    }
}
