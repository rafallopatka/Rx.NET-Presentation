using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11_Merge
{
    class Program
    {
        static void Main(string[] args)
        {
            IObservable<string> observable1 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(index => $"Observable1: #{index}");

            IObservable<string> observable2 = Observable.Interval(TimeSpan.FromSeconds(4))
                .Select(index => $"Observable2: #{index}");

            IObservable<string> observable3 = observable1.Merge(observable2);
            IDisposable subscription = observable3.Subscribe(Console.WriteLine);

            Console.ReadLine();
        }
    }
}
