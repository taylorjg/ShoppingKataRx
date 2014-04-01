using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Code
{
    public class Discounter
    {
        public IObservable<Tuple<string, int>> DiscountSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var itemCounter = new ItemCounter();
            return sequenceOfItems.Select(item => DiscountItem(item, itemCounter));
        }

        private static readonly IDictionary<char, Tuple<int, string, int>> Discounts =
            new Dictionary<char, Tuple<int, string, int>>
                {
                    {'A', Tuple.Create(3, "3 'A's", -20)},
                    {'B', Tuple.Create(2, "2 'B's", -15)}
                };

        private static readonly Tuple<string, int> NoDiscount = Tuple.Create(string.Empty, 0);

        private static Tuple<string, int> DiscountItem(char item, ItemCounter itemCounter)
        {
            if (Discounts.ContainsKey(item))
            {
                var discount = Discounts[item];
                var newItemCount = itemCounter.IncrementItemCountFor(item);
                if (newItemCount % discount.Item1 == 0)
                {
                    return Tuple.Create(discount.Item2, discount.Item3);
                }
            }

            return NoDiscount;
        }
    }
}
