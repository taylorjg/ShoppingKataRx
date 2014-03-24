﻿using System;
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
                                      : CreateSequenceOfItemsOverAsyncConsoleReadLoop();

            var checkout = new Checkout();
            var total = 0;
            checkout.ProcessSequenceOfItems(sequenceOfItems, totalDelta =>
                {
                    Log("totalDelta: {0}", totalDelta);
                    total += totalDelta;
                });
            checkout.Reset();
            Console.WriteLine("Total = {0}.", total);
        }

        private static IObservable<string> CreateSequenceOfItemsOverCommandLineArgument(string listOfItemsFromCommandLine)
        {
            using (new LogEntryExit("CreateSequenceOfItemsOverCommandLineArgument"))
            {
                var sequenceOfItems = new ReplaySubject<string>();
                foreach (var item in listOfItemsFromCommandLine.Select(c => Convert.ToString(c)))
                {
                    sequenceOfItems.OnNext(item);
                }
                sequenceOfItems.OnCompleted();
                return sequenceOfItems;
            }
        }

        private static IObservable<string> CreateSequenceOfItemsOverAsyncConsoleReadLoop()
        {
            using (new LogEntryExit("CreateSequenceOfItemsOverAsyncConsoleReadLoop"))
            {
                return Observable.Create<string>(observer =>
                {
                    using (new LogEntryExit("Observable.Create()'s subscribe lambda"))
                    {
                        Log("Calling ConsoleKeyPressReadLoop()...");
                        ConsoleKeyPressReadLoop(observer);
                        Log("...returned from ConsoleKeyPressReadLoop()");
                        return () => Log("Inside the subscription dispose action");
                    }
                });
            }
        }

        private async static void ConsoleKeyPressReadLoop(IObserver<string> observer)
        {
            using (new LogEntryExit("ConsoleKeyPressReadLoop"))
            {
                for (; ; )
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
