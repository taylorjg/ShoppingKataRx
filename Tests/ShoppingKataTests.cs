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
            var sequenceOfItems = new Subject<string>();
            var checkout = new Checkout(sequenceOfItems, newTotal => total = newTotal);

            // Act
            sequenceOfItems.OnNext("A");
            sequenceOfItems.OnCompleted();

            // Assert
            Assert.That(total, Is.EqualTo(50));
        }
    }
}
