using System;

namespace SimplePipeline.Tests.Filters
{
    public class Base64StringFilter : IFilter<String, Byte[]>, IFilter<Byte[], String>
    {
        public String Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return Convert.ToBase64String(input);
        }

        public Byte[] Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return Convert.FromBase64String(input);
        }
    }
}