﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using Code;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class CheckoutTests
    {
        [Test]
        public void NoItemsGivesATotalOfZero()
        {
            CommonTestImplementationForCalculatedTotal("", 0);
        }

        [TestCase("A", 50)]
        [TestCase("B", 30)]
        [TestCase("C", 20)]
        [TestCase("D", 15)]
        [TestCase("a", 50)]
        [TestCase("b", 30)]
        [TestCase("c", 20)]
        [TestCase("d", 15)]
        public void SingleItemHasTheCorrectPrice(string items, int expectedTotal)
        {
            CommonTestImplementationForCalculatedTotal(items, expectedTotal);
        }

        [TestCase("AAA", 150 - 20)]
        [TestCase("aaa", 150 - 20)]
        [TestCase("aAa", 150 - 20)]
        [TestCase("BB", 60 - 15)]
        [TestCase("bb", 60 - 15)]
        [TestCase("bB", 60 - 15)]
        public void OneDiscountIsAppliedWhenGivenOneTriggerQuantity(string items, int expectedTotal)
        {
            CommonTestImplementationForCalculatedTotal(items, expectedTotal);
        }

        [TestCase("AAAAAA", (150 - 20) * 2)]
        [TestCase("AAA AAA", (150 - 20) * 2)]
        [TestCase("aaa AAA", (150 - 20) * 2)]
        [TestCase("BBBB", (60 - 15) * 2)]
        [TestCase("BB BB", (60 - 15) * 2)]
        [TestCase("bb BB", (60 - 15) * 2)]
        public void MultipleDiscountsAreAppliedWhenGivenMultipleTriggerQuantities(string items, int expectedTotal)
        {
            CommonTestImplementationForCalculatedTotal(items, expectedTotal);
        }

        [TestCase("AAABB", (150 - 20) + (60 - 15))]
        [TestCase("AAA BB", (150 - 20) + (60 - 15))]
        [TestCase("aaa bb", (150 - 20) + (60 - 15))]
        [TestCase("aaa BB", (150 - 20) + (60 - 15))]
        public void MultipleDifferentDiscountsAreAppliedWhenGivenMultipleDifferentTriggerQuantities(string items, int expectedTotal)
        {
            CommonTestImplementationForCalculatedTotal(items, expectedTotal);
        }

        [TestCase("AAAA", (150 - 20) + 50)]
        [TestCase("AAA A", (150 - 20) + 50)]
        [TestCase("aaa a", (150 - 20) + 50)]
        [TestCase("aaa A", (150 - 20) + 50)]
        [TestCase("BBB", (60 - 15) + 30)]
        [TestCase("BB B", (60 - 15) + 30)]
        [TestCase("bb b", (60 - 15) + 30)]
        [TestCase("bb B", (60 - 15) + 30)]
        public void OneDiscountIsAppliedWhenGivenOneTriggerQuantityPlusOne(string items, int expectedTotal)
        {
            CommonTestImplementationForCalculatedTotal(items, expectedTotal);
        }

        [Test]
        public void OutputSequenceItemsAreCorrect()
        {
            // Arrange
            var inputSequence = CreateInputSequenceOfItems("AABA");
            var outputSequenceItems = new List<Tuple<string, int, int>>();

            // Act
            var onErrorEvent = new ManualResetEventSlim(false);
            var onCompletedEvent = new ManualResetEventSlim(false);
            var checkout = new Checkout();
            var outputSequence = checkout.ProcessSequenceOfItems(inputSequence);
            outputSequence.Subscribe(outputSequenceItems.Add, _ => onErrorEvent.Set(), onCompletedEvent.Set);
            WaitHandle.WaitAny(new[] { onErrorEvent.WaitHandle, onCompletedEvent.WaitHandle });

            // Assert
            Assert.That(outputSequenceItems.Count, Is.EqualTo(5));
            Assert.That(outputSequenceItems[0], Is.EqualTo(Tuple.Create("A", 50, 50)));
            Assert.That(outputSequenceItems[1], Is.EqualTo(Tuple.Create("A", 50, 100)));
            Assert.That(outputSequenceItems[2], Is.EqualTo(Tuple.Create("B", 30, 130)));
            Assert.That(outputSequenceItems[3], Is.EqualTo(Tuple.Create("A", 50, 180)));
            Assert.That(outputSequenceItems[4], Is.EqualTo(Tuple.Create("3 'A's", -20, 160)));
        }

        private static void CommonTestImplementationForCalculatedTotal(string items, int expectedTotal)
        {
            CommonTestImplementationForCalculatedTotalWaitingOnEvents(items, expectedTotal);
            CommonTestImplementationForCalculatedTotalWaitingOnATask(items, expectedTotal);
        }

        private static void CommonTestImplementationForCalculatedTotalWaitingOnEvents(string items, int expectedTotal)
        {
            // Arrange
            var inputSequence = CreateInputSequenceOfItems(items);
            var total = 0;

            // Act
            var onErrorEvent = new ManualResetEventSlim(false);
            var onCompletedEvent = new ManualResetEventSlim(false);
            var checkout = new Checkout();
            var outputSequence = checkout.ProcessSequenceOfItems(inputSequence);
            outputSequence.Subscribe(x => total = x.Item3, _ => onErrorEvent.Set(), onCompletedEvent.Set);
            var waitResultIndex = WaitHandle.WaitAny(new[]
                {
                    onErrorEvent.WaitHandle, // index 0
                    onCompletedEvent.WaitHandle // index 1
                });

            // Assert
            Assert.That(waitResultIndex, Is.EqualTo(1));
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        private static void CommonTestImplementationForCalculatedTotalWaitingOnATask(string items, int expectedTotal)
        {
            // Arrange
            var inputSequence = CreateInputSequenceOfItems(items);
            var total = 0;

            // Act
            var checkout = new Checkout();
            var outputSequence = checkout.ProcessSequenceOfItems(inputSequence);
            var task = outputSequence.LastOrDefaultAsync().ToTask();
            task.Wait();
            if (task.Result != null)
            {
                total = task.Result.Item3;
            }

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        private static IObservable<char> CreateInputSequenceOfItems(string items)
        {
            return items.ToObservable();
        }
    }
}
