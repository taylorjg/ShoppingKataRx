using System;
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
        public void SingleItemHasTheCorrectPrice(string itemsString, int expectedTotal)
        {
            // Arrange
            var sequenceOfItems = BuildSequenceOfItems(itemsString);

            // Act
            var checkout = new Checkout();
            var total = checkout.ProcessSequenceOfItems(sequenceOfItems);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        private static IObservable<char> BuildSequenceOfItems(string itemsString)
        {
            return itemsString.ToObservable();
        }
    }
}
