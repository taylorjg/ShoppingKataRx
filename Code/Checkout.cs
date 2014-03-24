using System;

namespace Code
{
    public class Checkout
    {
        private IDisposable _subscription;

        public void ProcessSequenceOfItems(IObservable<string> sequenceOfItems, Action<int> onTotalDelta)
        {
            _subscription = sequenceOfItems.Subscribe(
                _ => onTotalDelta(50),
                _ => { /* onError */ },
                () => { /* onCompleted */ });
        }

        public void Reset()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }
    }
}
