using System;

namespace SimplePipeline.Tests.Filters
{
    public class TrimFilter : IFilter<String, String>
    {
        private readonly Char[] trimChars;

        public TrimFilter() : this(Array.Empty<Char>()) { }

        public TrimFilter(params Char[] trimChars)
        {
            this.trimChars = trimChars ?? throw new ArgumentNullException(nameof(trimChars));
        }

        public String Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return input.Trim(trimChars);
        }
    }
}