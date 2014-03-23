using System;

namespace Code
{
    public class Checkout
    {
        public Checkout(IObservable<string> scannedItems, Action<int> onItem)
        {
            var subscription = scannedItems.Subscribe(
                item => onItem(50),
                () => { });
            //subscription.Dispose();
        }
    }
}
