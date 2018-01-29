using System;
using SimplePipeline;

namespace ConsoleTest
{
    public class TrimFilter : IFilter<String, String>
    {
        public String Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return input.Trim();
        }
    }
}