using System;

namespace Code
{
    public class Checkout
    {
        public int ProcessSequenceOfItems(IObservable<char> sequenceOfItems)
        {
            var total = 0;

            // 2 subscribers
            // - to lookup price => sequence of price (int)
            //  - Pricer
            // - to apply discount => sequence of discount (int)
            //  - Discounter + ItemCounter
            var pricer = new Pricer();
            var prices = pricer.PriceSequenceOfItems(sequenceOfItems);
            var discounter = new Discounter();

            // merge the 2 sequences of prices
            // sum all the values together
            // Merge, Sum
            // - Totaller
            //var totaller = new Totaller();

            var subscription = prices.Subscribe(price => total += price);

            // The observables that I have used so far (in unit tests and the
            // console app) seem to block when I call Subscribe above. But I
            // guess this might not necessarily be the case. How should I
            // handle disposing of the subscription more generally ?
            subscription.Dispose();

            return total;
        }
    }
}
