using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Code;

namespace App
{
    internal class Program
    {
        private static bool _enableLogging;

        private static void Main(string[] args)
        {
            _enableLogging = (args.LastOrDefault(arg => arg == "-log") != null);

            var listOfItemsFromCommandLine = args.FirstOrDefault(arg => !arg.StartsWith("-"));
            var sequenceOfItems = (listOfItemsFromCommandLine != null)
                                      ? CreateSequenceOfItemsOverCommandLineArgument(listOfItemsFromCommandLine)
                                      : CreateSequenceOfItemsOverConsoleReadLoop();

            var checkout = new Checkout();
            var total = 0;
            checkout.ProcessSequenceOfItems(sequenceOfItems, totalDelta =>
                {
                    total += totalDelta;
                    Console.WriteLine("totalDelta: {0}", totalDelta);
                    Console.WriteLine("total: {0}", total);
                });
            checkout.Reset();
            Console.WriteLine("Total = {0}.", total);
        }

        private static IObservable<string> CreateSequenceOfItemsOverCommandLineArgument(string listOfItemsFromCommandLine)
        {
            using (new LogEntryExit("CreateSequenceOfItemsOverCommandLineArgument"))
            {
                var sequenceOfItems = new ReplaySubject<string>();
                foreach (var item in listOfItemsFromCommandLine
                    .Where(c => !Char.IsWhiteSpace(c))
                    .Select(c => Convert.ToString(c)))
                {
                    sequenceOfItems.OnNext(item);
                }
                sequenceOfItems.OnCompleted();
                return sequenceOfItems;
            }
        }

        private static IObservable<string> CreateSequenceOfItemsOverConsoleReadLoop()
        {
            using (new LogEntryExit("CreateSequenceOfItemsOverConsoleReadLoop"))
            {
                return Observable.Create<string>(observer =>
                {
                    using (new LogEntryExit("Observable.Create()'s subscribe lambda"))
                    {
                        Log("Calling ConsoleReadLoop()...");
                        ConsoleReadLoop(observer);
                        Log("...returned from ConsoleReadLoop()");
                        return () => Log("Inside the subscription dispose action");
                    }
                });
            }
        }

        private static void ConsoleReadLoop(IObserver<string> observer)
        {
            using (new LogEntryExit("ConsoleReadLoop"))
            {
                for (; ; )
                {
                    Log("Calling Console.In.ReadLine()");
                    var line = Console.In.ReadLine();
                    Log("...after await Console.In.ReadLine()");

                    if (string.IsNullOrEmpty(line))
                    {
                        Log("Calling observer.OnCompleted() due to line being null or empty");
                        observer.OnCompleted();
                        return;
                    }

                    foreach (var keyPress in line)
                    {
                        if (Char.IsWhiteSpace(keyPress))
                        {
                            continue;
                        }

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
        }

        private static void Log(string format, params object[] args)
        {
            if (_enableLogging)
            {
                Console.WriteLine(format, args);
            }
        }

        private class LogEntryExit : IDisposable
        {
            private readonly string _name;

            public LogEntryExit(string name)
            {
                _name = name;
                Log("Entering {0}", _name);
            }

            public void Dispose()
            {
                Log("Leaving {0}", _name);
            }
        }
    }
}
