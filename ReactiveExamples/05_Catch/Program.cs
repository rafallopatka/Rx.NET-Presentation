using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace _05_Catch
{
    class Program
    {
        static void Main(string[] args)
        {
            Observable.Create<int>(observer =>
            {
                observer.OnNext(1);
                throw new InvalidOperationException("InvalidOperationException");
                observer.OnNext(2);
                return Disposable.Empty;
            })
            .Catch<int, InvalidOperationException>(e =>
            {
                Console.WriteLine($"Exception {e.GetType()}");
                return Observable.Return(-1);
            })
            .Finally(() => Console.WriteLine($"Finally"))
            .Subscribe(onNext: item => Console.WriteLine($"OnNext: {item}"),
                       onError: exception => Console.WriteLine($"OnError: {exception}"),
                       onCompleted: () => Console.WriteLine("Completed"));

        }
    }
}
