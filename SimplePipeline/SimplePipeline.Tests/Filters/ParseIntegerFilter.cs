using System;

namespace SimplePipeline.Tests.Filters
{
    public class ParseIntegerFilter : IFilter<String, Int32>
    {
        public Int32 Execute(String input)
        {
            return Int32.Parse(input);
        }
    }
}