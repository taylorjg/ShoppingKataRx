using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
        public async void SingleItemHasTheCorrectPrice(string itemsString, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems(itemsString);

            // Act
            var checkout = new Checkout();
            var total = await checkout.ProcessSequenceOfItems(sequenceOfItems);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase("AAA", 150 - 20)]
        [TestCase("aaa", 150 - 20)]
        [TestCase("aAa", 150 - 20)]
        [TestCase("BB", 60 - 15)]
        [TestCase("bb", 60 - 15)]
        [TestCase("bB", 60 - 15)]
        public async void OneDiscountIsAppliedWhenGivenOneTriggerQuantity(string itemsString, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems(itemsString);

            // Act
            var checkout = new Checkout();
            var total = await checkout.ProcessSequenceOfItems(sequenceOfItems);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase("AAAAAA", (150 - 20) * 2)]
        [TestCase("AAA AAA", (150 - 20) * 2)]
        [TestCase("aaa AAA", (150 - 20) * 2)]
        [TestCase("BBBB", (60 - 15) * 2)]
        [TestCase("BB BB", (60 - 15) * 2)]
        [TestCase("bb BB", (60 - 15) * 2)]
        public async void MultipleDiscountsAreAppliedWhenGivenMultipleTriggerQuantities(string itemsString, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems(itemsString);

            // Act
            var checkout = new Checkout();
            var total = await checkout.ProcessSequenceOfItems(sequenceOfItems);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase("AAABB", (150 - 20) + (60 - 15))]
        [TestCase("AAA BB", (150 - 20) + (60 - 15))]
        [TestCase("aaa bb", (150 - 20) + (60 - 15))]
        [TestCase("aaa BB", (150 - 20) + (60 - 15))]
        public async void MultipleDifferentDiscountsAreAppliedWhenGivenMultipleDifferentTriggerQuantities(string itemsString, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems(itemsString);

            // Act
            var checkout = new Checkout();
            var total = await checkout.ProcessSequenceOfItems(sequenceOfItems);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase("AAAA", (150 - 20) + 50)]
        [TestCase("AAA A", (150 - 20) + 50)]
        [TestCase("aaa a", (150 - 20) + 50)]
        [TestCase("aaa A", (150 - 20) + 50)]
        [TestCase("BBB", (60 - 15) + 30)]
        [TestCase("BB B", (60 - 15) + 30)]
        [TestCase("bb b", (60 - 15) + 30)]
        [TestCase("bb B", (60 - 15) + 30)]
        public async void OneDiscountIsAppliedWhenGivenOneTriggerQuantityPlusOne(string itemsString, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems(itemsString);

            // Act
            var checkout = new Checkout();
            var total = await checkout.ProcessSequenceOfItems(sequenceOfItems);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [Test]
        public void OnTotalChangeActionIsCalledCorrectly()
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems("AABA");
            var callbacks = new List<Tuple<string, int, int>>();

            // Act
            var checkout = new Checkout();
            checkout
                .ProcessSequenceOfItems(
                    sequenceOfItems,
                    (item, totalDelta, runningTotal) => callbacks.Add(Tuple.Create(item, totalDelta, runningTotal)))
                .Wait();

            // Assert
            Assert.That(callbacks.Count, Is.EqualTo(5));
            Assert.That(callbacks[0], Is.EqualTo(Tuple.Create("A", 50, 50)));
            Assert.That(callbacks[1], Is.EqualTo(Tuple.Create("A", 50, 100)));
            Assert.That(callbacks[2], Is.EqualTo(Tuple.Create("B", 30, 130)));
            Assert.That(callbacks[3], Is.EqualTo(Tuple.Create("A", 50, 180)));
            Assert.That(callbacks[4].Item2, Is.EqualTo(-20));
            Assert.That(callbacks[4].Item3, Is.EqualTo(160));
        }

        private static IObservable<char> BuildSequenceOfItems(string itemsString)
        {
            return itemsString.ToObservable();
        }
    }
}
