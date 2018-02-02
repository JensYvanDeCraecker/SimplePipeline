using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Example.Filters
{
    public class OrderDescendingFilter<T, TOrder> : IFilter<IEnumerable<T>, IEnumerable<T>>
    {
        private readonly Func<T, TOrder> orderFunc;

        public OrderDescendingFilter(Func<T, TOrder> orderFunc)
        {
            this.orderFunc = orderFunc ?? throw new ArgumentNullException(nameof(orderFunc));
        }

        public IEnumerable<T> Execute(IEnumerable<T> input)
        {
            return input.OrderByDescending(orderFunc);
        }
    }
}