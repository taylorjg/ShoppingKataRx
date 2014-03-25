using System;
using System.Threading.Tasks;

namespace Code
{
    public class Checkout
    {
        public async Task<int> ProcessSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var pricer = new Pricer();
            var prices = pricer.PriceSequenceOfItems(sequenceOfItems);

            var discounter = new Discounter();
            var discounts = discounter.DiscountSequenceOfItems(sequenceOfItems);

            var totaller = new Totaller();
            return await totaller.TotalPricesAndDiscounts(prices, discounts);
        }
    }
}
