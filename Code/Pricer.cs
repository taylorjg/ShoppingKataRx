using System;
using System.Reactive.Linq;

namespace Code
{
    public class Pricer
    {
        public IObservable<Tuple<string, int>> PriceSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            return sequenceOfItems.Select(x => Tuple.Create(string.Format("{0}", x), LookupPrice(x)));
        }

        private static int LookupPrice(char item)
        {
            switch (item)
            {
                case 'A':
                    return 50;
                case 'B':
                    return 30;
                case 'C':
                    return 20;
                case 'D':
                    return 15;
                default:
                    throw new InvalidOperationException(string.Format("Unrecognised basket item, '{0}'.", item));
            }
        }
    }
}
