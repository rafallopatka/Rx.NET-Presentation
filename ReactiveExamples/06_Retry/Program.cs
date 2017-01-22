using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_Retry
{
    class Program
    {
        static void Main(string[] args)
        {
            Observable.Create<int>(observer =>
            {
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);
                throw new InvalidOperationException("InvalidOperationException");
                return Disposable.Empty;
            })
            .Retry(2)
            .Subscribe(onNext: item => Console.WriteLine($"OnNext: {item}"),
                       onError: exception => Console.WriteLine($"OnError: {exception.GetType()}"),
                       onCompleted: () => Console.WriteLine("Completed"));
        }
    }
}
