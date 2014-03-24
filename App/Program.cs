using System;
using System.Linq;
using System.Reactive.Subjects;
using Code;

namespace App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IObservable<string> sequenceOfItems = null;

            if (args.Length > 0)
            {
                if (args.Length > 1)
                {
                    Usage();
                }

                var rs = new ReplaySubject<string>();
                foreach (var item in args[0].Select(c => Convert.ToString(c)))
                {
                    rs.OnNext(item);
                }
                rs.OnCompleted();
                sequenceOfItems = rs;
            }
            else
            {
                // Need to create an observable to read console input
                // Can we read key presses asynchronously via Console.In.ReadAsync() ?
                //var buffer = new char[1];
                //Console.In.ReadAsync(buffer, 0, buffer.Length);
                // Q,q => OnCompleted()
                // anythin else => OnNext()
            }

            var checkout = new Checkout();
            var total = 0;
            checkout.ProcessSequenceOfItems(sequenceOfItems, newTotal => total += newTotal);
            checkout.Reset();
            Console.WriteLine("Total = {0}.", total);
        }

        private static void Usage()
        {
            Console.Error.WriteLine("App [ <list of items> ]");
            Console.Error.WriteLine("");
            Console.Error.WriteLine("\tApp AABC");
            Environment.Exit(1);
        }
    }
}
