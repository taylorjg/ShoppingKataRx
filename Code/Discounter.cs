using System;
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

        private static readonly Tuple<string, int> NoDiscount = Tuple.Create(string.Empty, 0);

        private static Tuple<string, int> DiscountItem(char item, ItemCounter itemCounter)
        {
            int triggerQuantity;
            int discount;

            switch (item)
            {
                case 'A':
                    triggerQuantity = 3;
                    discount = 20;
                    break;

                case 'B':
                    triggerQuantity = 2;
                    discount = 15;
                    break;

                default:
                    return NoDiscount;
            }

            var newItemCount = itemCounter.IncrementItemCountForItem(item);
            if (newItemCount % triggerQuantity == 0)
            {
                var discountDescription = string.Format("{0} '{1}'s", triggerQuantity, item);
                return Tuple.Create(discountDescription, -discount);
            }

            return NoDiscount;
        }
    }
}
