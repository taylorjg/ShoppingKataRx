using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Code;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class CheckoutTests
    {
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
            CommonTestImplementation(items, expectedTotal);
        }

        [TestCase("AAA", 150 - 20)]
        [TestCase("aaa", 150 - 20)]
        [TestCase("aAa", 150 - 20)]
        [TestCase("BB", 60 - 15)]
        [TestCase("bb", 60 - 15)]
        [TestCase("bB", 60 - 15)]
        public void OneDiscountIsAppliedWhenGivenOneTriggerQuantity(string items, int expectedTotal)
        {
            CommonTestImplementation(items, expectedTotal);
        }

        [TestCase("AAAAAA", (150 - 20) * 2)]
        [TestCase("AAA AAA", (150 - 20) * 2)]
        [TestCase("aaa AAA", (150 - 20) * 2)]
        [TestCase("BBBB", (60 - 15) * 2)]
        [TestCase("BB BB", (60 - 15) * 2)]
        [TestCase("bb BB", (60 - 15) * 2)]
        public void MultipleDiscountsAreAppliedWhenGivenMultipleTriggerQuantities(string items, int expectedTotal)
        {
            CommonTestImplementation(items, expectedTotal);
        }

        [TestCase("AAABB", (150 - 20) + (60 - 15))]
        [TestCase("AAA BB", (150 - 20) + (60 - 15))]
        [TestCase("aaa bb", (150 - 20) + (60 - 15))]
        [TestCase("aaa BB", (150 - 20) + (60 - 15))]
        public void MultipleDifferentDiscountsAreAppliedWhenGivenMultipleDifferentTriggerQuantities(string items, int expectedTotal)
        {
            CommonTestImplementation(items, expectedTotal);
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
            CommonTestImplementation(items, expectedTotal);
        }

        [Test]
        public void OnTotalChangeActionIsCalledCorrectly()
        {
            // Arrange
            var sequenceOfItems = CreateSequenceOfItems("AABA");
            var callbacks = new List<Tuple<string, int, int>>();

            // Act
            var checkout = new Checkout();
            checkout
                .ProcessSequenceOfItems(
                    sequenceOfItems,
                    (description, value, runningTotal) => callbacks.Add(Tuple.Create(description, value, runningTotal)))
                .Wait();

            // Assert
            Assert.That(callbacks.Count, Is.EqualTo(5));
            Assert.That(callbacks[0], Is.EqualTo(Tuple.Create("A", 50, 50)));
            Assert.That(callbacks[1], Is.EqualTo(Tuple.Create("A", 50, 100)));
            Assert.That(callbacks[2], Is.EqualTo(Tuple.Create("B", 30, 130)));
            Assert.That(callbacks[3], Is.EqualTo(Tuple.Create("A", 50, 180)));
            Assert.That(callbacks[4], Is.EqualTo(Tuple.Create("3 'A's", -20, 160)));
        }

        private static void CommonTestImplementation(string items, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = CreateSequenceOfItems(items);

            // Act
            var checkout = new Checkout();
            var task = checkout.ProcessSequenceOfItems(sequenceOfItems);
            task.Wait();
            var total = task.Result;

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        private static IObservable<char> CreateSequenceOfItems(string items)
        {
            return items.ToObservable();
        }
    }
}
