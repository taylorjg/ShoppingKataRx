using System;
using System.Reactive.Linq;

namespace Code
{
    public class Discounter
    {
        public IObservable<int> DiscountSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var itemCounter = new ItemCounter();
            return sequenceOfItems.Select(item =>
                {
                    var discount = 0;
                    var newItemCount = itemCounter.IncrementItemCountForItem(item);

                    switch (item)
                    {
                        case 'A':
                            discount = (newItemCount % 3 == 0) ? -20 : 0;
                            break;

                        case 'B':
                            discount = (newItemCount % 2 == 0) ? -15 : 0;
                            break;
                    }

                    return discount;
                });
        }
    }
}
