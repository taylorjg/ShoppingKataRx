using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Code
{
    public class Pricer
    {
        public IObservable<Tuple<string, int>> PriceSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            return sequenceOfItems.Select(x => Tuple.Create(string.Format("{0}", x), LookupPrice(x)));
        }

        private static readonly IDictionary<char, int> PriceList = new Dictionary<char, int>
            {
                {'A', 50},
                {'B', 30},
                {'C', 20},
                {'D', 15}
            };

        private static int LookupPrice(char item)
        {
            try
            {
                return PriceList[item];
            }
            catch (KeyNotFoundException)
            {
                // TODO: decide how best to handle unknown basket items. For now, just throw an exception.
                throw new UnknownBasketItemException(item);
            }
        }
    }
}
