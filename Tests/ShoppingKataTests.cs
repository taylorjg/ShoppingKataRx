using System;
using System.Reactive.Subjects;
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

        private IObservable<string> BuildSequenceOfItems(string items)
        {
            var sequenceOfItems = new ReplaySubject<string>();
            foreach (var item in items)
            {
                sequenceOfItems.OnNext(Convert.ToString(item));
            }
            sequenceOfItems.OnCompleted();
            return sequenceOfItems;
        }
    }
}
