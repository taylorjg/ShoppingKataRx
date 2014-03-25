using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Code
{
    public class Totaller
    {
        public async Task<int> TotalPricesAndDiscounts(IObservable<int> prices, IObservable<int> discounts)
        {
            return await prices.Merge(discounts).Sum().FirstAsync();
        }
    }
}
