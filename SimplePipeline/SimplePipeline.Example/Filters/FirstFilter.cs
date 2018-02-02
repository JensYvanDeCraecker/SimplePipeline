using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Example.Filters
{
    public class FirstFilter<T> : IFilter<IEnumerable<T>, T>
    {
        public T Execute(IEnumerable<T> input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return input.First();
        }
    }
}