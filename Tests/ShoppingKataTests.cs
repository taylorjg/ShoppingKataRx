using System;
using System.Linq;
using System.Reactive.Linq;
using Code;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class ShoppingKataTests
    {
        [Test]
        public void SingleItemHasTheCorrectPrice()
        {
            // Arrange
            var total = 0;
            var sequenceOfItems = BuildSequenceOfItems("A");

            // Act
            var checkout = new Checkout();
            checkout.ProcessSequenceOfItems(sequenceOfItems, totalDelta => total += totalDelta);
            checkout.Reset();

            // Assert
            Assert.That(total, Is.EqualTo(50));
        }

        private static IObservable<string> BuildSequenceOfItems(string items)
        {
            return items.Select(c => Convert.ToString(c)).ToObservable();
        }
    }
}
