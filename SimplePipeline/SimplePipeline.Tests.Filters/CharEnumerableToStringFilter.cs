using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Tests.Filters
{
    public class CharEnumerableToStringFilter : IFilter<IEnumerable<Char>, string>
    {
        private readonly IFilter<IEnumerable<Char>, Char[]> toArrayFilter = new CharEnumerableToArray();
        public String Execute(IEnumerable<Char> input)
        {
            return new String(toArrayFilter.Execute(input));
        }
    }
}