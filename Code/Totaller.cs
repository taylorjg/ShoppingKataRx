using System;
using System.Reactive.Linq;

namespace Code
{
    public class Totaller
    {
        public IObservable<Tuple<string, int, int>> TotalPricesAndDiscounts2(
            IObservable<Tuple<string, int>> prices,
            IObservable<Tuple<string, int>> discounts)
        {
            var seed = Tuple.Create(string.Empty, 0, 0);

            return prices
                .Merge(discounts)
                .Scan(seed, (accumulator, x) =>
                    {
                        var description = x.Item1;
                        var value = x.Item2;
                        var previousTotal = accumulator.Item3;
                        var runningTotal = previousTotal + value;
                        return Tuple.Create(description, value, runningTotal);
                    });
        }
    }
}
