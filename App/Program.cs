using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Code;

namespace App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Usage();
            }

            var sequenceOfItems = (args.Length == 1)
                                      ? CreateSequenceOfItemsOverCommandLineArgument(args)
                                      : CreateSequenceOfItemsOverAsyncConsoleReadLoop();

            var checkout = new Checkout();
            var total = 0;
            checkout.ProcessSequenceOfItems(sequenceOfItems, newTotal => total += newTotal);
            checkout.Reset();
            Console.WriteLine("Total = {0}.", total);
        }

        private static IObservable<string> CreateSequenceOfItemsOverCommandLineArgument(IList<string> args)
        {
            var sequenceOfItems = new ReplaySubject<string>();
            foreach (var item in args[0].Select(c => Convert.ToString(c)))
            {
                sequenceOfItems.OnNext(item);
            }
            sequenceOfItems.OnCompleted();
            return sequenceOfItems;
        }

        private static IObservable<string> CreateSequenceOfItemsOverAsyncConsoleReadLoop()
        {
            return Observable.Create<string>(observer =>
                {
                    Log("Calling ConsoleKeyPressReadLoop()...");
                    ConsoleKeyPressReadLoop(observer);
                    Log("...returned from ConsoleKeyPressReadLoop()");
                    return () => Log("Inside the subscription dispose action");
                });
        }

        private async static void ConsoleKeyPressReadLoop(IObserver<string> observer)
        {
            for (;;)
            {
                Log("Calling await Console.In.ReadLineAsync()");
                var line = await Console.In.ReadLineAsync();
                Log("...after await Console.In.ReadLineAsync() - line.Length: {0}", line.Length);

                if (line.Length == 0)
                {
                    Log("Calling observer.OnCompleted() due to empty line");
                    observer.OnCompleted();
                    return;
                }

                foreach (var keyPress in line)
                {
                    if (keyPress == 'Q' || keyPress == 'q')
                    {
                        Log("Calling observer.OnCompleted() due to '{0}'", keyPress);
                        observer.OnCompleted();
                        return;
                    }

                    Log("Calling observer.OnNext('{0}')", keyPress);
                    observer.OnNext(Convert.ToString(keyPress));
                }
            }
        }

        private static void Usage()
        {
            Console.Error.WriteLine("App [ <list of items> ]");
            Console.Error.WriteLine("    e.g. App AABC");
            Environment.Exit(1);
        }

        private static void Log(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
