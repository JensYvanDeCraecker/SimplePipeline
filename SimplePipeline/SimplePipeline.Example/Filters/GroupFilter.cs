using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Example.Filters
{
    public class GroupFilter<T, TKey> : IFilter<IEnumerable<T>, IEnumerable<IGrouping<TKey, T>>>
    {
        private readonly Func<T, TKey> selector;

        public GroupFilter(Func<T, TKey> selector)
        {
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        public IEnumerable<IGrouping<TKey, T>> Execute(IEnumerable<T> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return input.GroupBy(selector);
        }
    }
}