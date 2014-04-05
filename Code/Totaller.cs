﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Code
{
    public class Totaller
    {
        public async Task<int> TotalPricesAndDiscounts(
            IObservable<Tuple<string, int>> prices,
            IObservable<Tuple<string, int>> discounts,
            Action<string, int, int> onTotalChange)
        {
            var runningTotal = 0;

            return await prices
                .Merge(discounts)
                .Do(x => onTotalChange(x.Item1, x.Item2, runningTotal += x.Item2))
                .Sum(x => x.Item2).FirstAsync();
        }
    }
}
