using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Tests.Filters
{
    public class EnumerableCountFilter<T> : IFilter<IEnumerable<T>, Int32>
    {
        public Int32 Execute(IEnumerable<T> input)
        {
            return input.Count();
        }
    }
}