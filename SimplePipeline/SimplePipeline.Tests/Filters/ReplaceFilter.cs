using System;

namespace SimplePipeline.Tests.Filters
{
    public class ReplaceFilter : IFilter<String, String>
    {
        private readonly Char newChar;
        private readonly Char oldChar;

        public ReplaceFilter(Char oldChar, Char newChar)
        {
            this.oldChar = oldChar;
            this.newChar = newChar;
        }

        public String Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return input.Replace(oldChar, newChar);
        }
    }
}