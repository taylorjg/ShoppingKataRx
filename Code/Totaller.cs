using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Code
{
    public class Totaller
    {
        public Task<int> TotalPricesAndDiscounts(
            IObservable<Tuple<string, int>> prices,
            IObservable<Tuple<string, int>> discounts,
            Action<string, int, int> onTotalChange)
        {
            return prices
                .Merge(discounts)
                .Scan(0, (oldTotal, x) =>
                    {
                        var description = x.Item1;
                        var value = x.Item2;
                        var newTotal = oldTotal + value;
                        onTotalChange(description, value, newTotal);
                        return newTotal;
                    })
                .ToTask();
        }

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
