using System;
using System.Reactive.Linq;

namespace Code
{
    public class Checkout
    {
        public IObservable<Tuple<string, int, int>> ProcessSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var cleanedHotSequenceOfItems = sequenceOfItems
                .Where(Char.IsLetter)
                .Select(Char.ToUpper)
                .Publish()
                .RefCount();

            var pricer = new Pricer();
            var prices = pricer.PriceSequenceOfItems(cleanedHotSequenceOfItems);

            var discounter = new Discounter();
            var discounts = discounter.DiscountSequenceOfItems(cleanedHotSequenceOfItems);

            var totaller = new Totaller();
            return totaller.TotalPricesAndDiscounts(prices, discounts);
        }
    }
}
