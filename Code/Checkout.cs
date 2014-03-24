using System;

namespace Code
{
    public class Checkout
    {
        public Checkout(IObservable<string> sequenceOfItems, Action<int> onTotalChange)
        {
            var subscription = sequenceOfItems.Subscribe(
                _ => onTotalChange(50),
                _ => { /* onError */ },
                () => { /* onCompleted */ });
        }
    }
}
