﻿using System;
using System.Collections.Generic;

namespace Code
{
    internal class ItemCounter
    {
        private readonly IDictionary<char, int> _itemCounts = new Dictionary<char, int>();

        public Tuple<char, int> IncrementItemCountForItem(char item)
        {
            var newItemCount = (_itemCounts.ContainsKey(item)) ? _itemCounts[item] : 0;
            newItemCount++;
            _itemCounts[item] = newItemCount;
            return Tuple.Create(item, newItemCount);
        }
    }
}
