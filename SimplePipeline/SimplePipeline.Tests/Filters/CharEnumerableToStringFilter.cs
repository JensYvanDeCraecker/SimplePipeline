using System;
using System.Collections.Generic;

namespace SimplePipeline.Tests.Filters
{
    public class CharEnumerableToStringFilter : IFilter<IEnumerable<Char>, String>
    {
        private readonly IFilter<IEnumerable<Char>, Char[]> toArrayFilter = new EnumerableToArrayFilter<Char>();

        public String Execute(IEnumerable<Char> input)
        {
            return new String(toArrayFilter.Execute(input));
        }
    }
}