using System;

namespace Code
{
    public class Checkout
    {
        public int ProcessSequenceOfItems(IObservable<string> sequenceOfItems)
        {
            var total = 0;
            var subscription = sequenceOfItems.Subscribe(
                item => total += LookupItem(item),
                _ => { /* onError */ },
                () => { /* onCompleted */ });
            subscription.Dispose();
            return total;
        }

        private int LookupItem(string item)
        {
            switch (item.ToUpper()[0])
            {
                case 'A':
                    return 50;
                case 'B':
                    return 30;
                case 'C':
                    return 20;
                case 'D':
                    return 15;
            }

            return 0;
        }
    }
}
