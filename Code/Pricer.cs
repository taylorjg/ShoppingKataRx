using System;
using System.Reactive.Linq;

namespace Code
{
    public class Pricer
    {
        public IObservable<int> PriceSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            return sequenceOfItems.Select(LookupPrice);
        }

        private static int LookupPrice(char item)
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
