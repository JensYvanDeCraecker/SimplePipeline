using System;

namespace SimplePipeline.Tests.Filters
{
    public class Base64String : IFilter<String, byte[]>, IFilter<Byte[], String>
    {
        public Byte[] Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return Convert.FromBase64String(input);
        }

        public String Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return Convert.ToBase64String(input);
        }
    }
}