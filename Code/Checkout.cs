using System;
using System.Reactive.Linq;
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

            //var totaller = new Totaller();
            //var subscription = prices.Subscribe(price => total += price);

            var total = await prices.Merge(discounts).Sum().FirstAsync();

            // The observables that I have used so far (in unit tests and the
            // console app) seem to block when I call Subscribe above. But I
            // guess this might not necessarily be the case. How should I
            // handle disposing of the subscription more generally ?
            // subscription.Dispose();

            return total;
        }
    }
}
