using System;
using System.Reactive.Linq;

namespace Code
{
    public class Checkout
    {
        public int ProcessSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var total = 0;

            var itemsAndCounts = sequenceOfItems.Select(c => Tuple.Create(c, 0));

            var subscription = itemsAndCounts.Subscribe(
                x => total += LookupItem(x.Item1),
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
