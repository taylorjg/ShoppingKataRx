using System;

namespace Code
{
    public class UnknownBasketItemException : Exception
    {
        public UnknownBasketItemException(char item)
            : base(string.Format("Unknown basket item, '{0}'.", item))
        {
            Item = item;
        }

        public char Item { get; private set; }
    }
}
