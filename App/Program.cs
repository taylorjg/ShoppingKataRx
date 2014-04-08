using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Code;

namespace App
{
    internal class Program
    {
        private static bool _enableLogging;

        private static void Main(string[] args)
        {
            try
            {
                _enableLogging = (args.LastOrDefault(arg => arg == "-log") != null);

                var listOfItemsFromCommandLine = args.FirstOrDefault(arg => !arg.StartsWith("-"));
                var sequenceOfItems = (listOfItemsFromCommandLine != null)
                                          ? CreateSequenceOfItemsOverCommandLineArgument(listOfItemsFromCommandLine)
                                          : CreateSequenceOfItemsOverConsoleReadLoop()
                                                .SubscribeOn(NewThreadScheduler.Default);

                Console.WriteLine();
                Console.WriteLine("Item\tValue\tRunning Total");
                Console.WriteLine("----\t-----\t-------------");
                Console.WriteLine();

                var onErrorEvent = new ManualResetEventSlim(false);
                var onCompleteEvent = new ManualResetEventSlim(false);
                var total = 0;

                var checkout = new Checkout();
                checkout.ProcessSequenceOfItems2(sequenceOfItems).Subscribe(
                    x =>
                        {
                            var description = x.Item1;
                            var value = x.Item2;
                            var runningTotal = x.Item3;
                            Console.WriteLine("{0}\t{1,5:N0}\t{2,13:N0}", description, value, runningTotal);
                            total = runningTotal;
                        },
                    _ => onErrorEvent.Set(),
                    onCompleteEvent.Set);

                Log("Before WaitHandle.WaitAny...");
                WaitHandle.WaitAny(new[]
                    {
                        onErrorEvent.WaitHandle,
                        onCompleteEvent.WaitHandle
                    });
                Log("...after WaitHandle.WaitAny...");

                Console.WriteLine();
                Console.WriteLine("Total = {0}", total);
            }
            catch (Exception ex)
            {
                ex = ex.InnerException ?? ex;
                if (!(ex is UnknownBasketItemException))
                {
                    throw;
                }
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(1);
            }
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
                        ConsoleReadLoop(observer);
                        return () => Log("Inside the subscription dispose action");
                    }
                });

                return sequenceOfItems;
            }
        }

        private static void ConsoleReadLoop(IObserver<char> observer)
        {
            using (new LogEntryExit("ConsoleReadLoop"))
            {
                for (; ; )
                {
                    Log("Calling Console.ReadLine()");
                    var line = Console.ReadLine();
                    Log("...after Console.ReadLine()");

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
                var formattedMessage = string.Format(format, args);
                var managedThreadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("[{0,5:N0}] {1}", managedThreadId, formattedMessage);
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
