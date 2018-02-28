using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Tests.Filters
{
    public class CharEnumerableToArray : IFilter<IEnumerable<Char>, Char[]>
    {
        public Char[] Execute(IEnumerable<Char> input)
        {
            return input.ToArray();
        }
    }
}