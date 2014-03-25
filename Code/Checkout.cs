using System;

namespace Code
{
    public class Checkout
    {
        public int ProcessSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var total = 0;
            var subscription = sequenceOfItems.Subscribe(
                item => total += LookupItem(item),
                _ => { /* onError */ },
                () => { /* onCompleted */ });
            subscription.Dispose();
            return total;
        }

        private static int LookupItem(char item)
        {
            switch (Char.ToUpper(item))
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

            throw new InvalidOperationException(string.Format("Unrecognised basket item, '{0}'.", item));
        }
    }
}
