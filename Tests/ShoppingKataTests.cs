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
            var scannedItems = new Subject<string>();
            var checkout = new Checkout(scannedItems, t => total = t);

            // Act
            scannedItems.OnNext("A");
            scannedItems.OnCompleted();

            // Assert
            Assert.That(total, Is.EqualTo(50));
        }
    }
}
