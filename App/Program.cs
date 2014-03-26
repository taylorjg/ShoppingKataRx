using System;
using System.Linq;
using System.Reactive.Linq;
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

            Console.WriteLine();
            Console.WriteLine("Item\tPrice\tRunning Total");
            Console.WriteLine("----\t-----\t-------------");
            Console.WriteLine();

            var checkout = new Checkout();
            var task = checkout.ProcessSequenceOfItems(sequenceOfItems, (item, totalDelta, runningTotal) => Console.WriteLine("{0}\t{1,5:N0}\t{2,13:N0}", item, totalDelta, runningTotal));

            task.Wait();
            var total = task.Result;

            Console.WriteLine();
            Console.WriteLine("Total = {0}", total);
        }

        private static IObservable<char> CreateSequenceOfItemsOverCommandLineArgument(string listOfItemsFromCommandLine)
        {
            using (new LogEntryExit("CreateSequenceOfItemsOverCommandLineArgument"))
            {
                return listOfItemsFromCommandLine.ToObservable();
            }
        }

        private static IObservable<char> CreateSequenceOfItemsOverConsoleReadLoop()
        {
            using (new LogEntryExit("CreateSequenceOfItemsOverConsoleReadLoop"))
            {
                var sequenceOfItems = Observable.Create<char>(observer =>
                {
                    using (new LogEntryExit("Observable.Create()'s subscribe lambda"))
                    {
                        Log("Calling ConsoleReadLoop()...");
                        ConsoleReadLoop(observer);
                        Log("...returned from ConsoleReadLoop()");
                        return () => Log("Inside the subscription dispose action");
                    }
                });
                var connectableObservable = sequenceOfItems.Publish();
                connectableObservable.Connect();
                return connectableObservable;
            }
        }

        private static void ConsoleReadLoop(IObserver<char> observer)
        {
            using (new LogEntryExit("ConsoleReadLoop"))
            {
                var consoleReadLoopThread = new System.Threading.Thread(() => ConsoleReadLoopInner(observer));
                consoleReadLoopThread.Start();
            }
        }

        private static void ConsoleReadLoopInner(IObserver<char> observer)
        {
            using (new LogEntryExit("ConsoleReadLoopInner"))
            {
                for (; ; )
                {
                    Log("Calling Console.ReadLine()");
                    var line = Console.ReadLine();
                    Log("...after await Console.ReadLine()");

                    if (string.IsNullOrEmpty(line))
                    {
                        Log("Calling observer.OnCompleted() due to line being null or empty");
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
                        observer.OnNext(keyPress);
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
