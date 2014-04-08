using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Code
{
    public class Checkout
    {
        public Task<int> ProcessSequenceOfItems(IObservable<char> sequenceOfItems, Action<string, int, int> onTotalChange = null)
        {
            var cleanedSequenceOfItems = sequenceOfItems
                .Where(Char.IsLetter)
                .Select(Char.ToUpper);

            var pricer = new Pricer();
            var prices = pricer.PriceSequenceOfItems(cleanedSequenceOfItems);

            var discounter = new Discounter();
            var discounts = discounter.DiscountSequenceOfItems(cleanedSequenceOfItems);

            var totaller = new Totaller();
            return totaller.TotalPricesAndDiscounts(prices, discounts, onTotalChange ?? ((_, __, ___) => { }));
        }

        public IObservable<Tuple<string, int, int>> ProcessSequenceOfItems2(IObservable<char> sequenceOfItems)
        {
            var cleanedSequenceOfItems = sequenceOfItems
                .Where(Char.IsLetter)
                .Select(Char.ToUpper);

            var pricer = new Pricer();
            var prices = pricer.PriceSequenceOfItems(cleanedSequenceOfItems);

            var discounter = new Discounter();
            var discounts = discounter.DiscountSequenceOfItems(cleanedSequenceOfItems);

            var totaller = new Totaller();
            return totaller.TotalPricesAndDiscounts2(prices, discounts);
        }
    }
}
