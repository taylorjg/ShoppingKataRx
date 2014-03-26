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

        private static Tuple<string, int> DiscountItem(char item, ItemCounter itemCounter)
        {
            var discount = 0;
            var triggerQuantity = 0;

            var newItemCount = itemCounter.IncrementItemCountForItem(item);

            switch (item)
            {
                case 'A':
                    triggerQuantity = 3;
                    discount = (newItemCount % triggerQuantity == 0) ? -20 : 0;
                    break;

                case 'B':
                    triggerQuantity = 2;
                    discount = (newItemCount % triggerQuantity == 0) ? -15 : 0;
                    break;
            }

            var discountDescription = (discount != 0) ? string.Format("{0} '{1}'s", triggerQuantity, item) : string.Empty;

            return Tuple.Create(discountDescription, discount);
        }
    }
}
