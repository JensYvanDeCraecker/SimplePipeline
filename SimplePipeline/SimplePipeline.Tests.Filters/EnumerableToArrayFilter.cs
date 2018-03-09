using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Tests.Filters
{
    public class EnumerableToArrayFilter<T> : IFilter<IEnumerable<T>, T[]>
    {
        public T[] Execute(IEnumerable<T> input)
        {
            return input.ToArray();
        }
    }
}