using System;
using System.Text;

namespace SimplePipeline.Tests.Filters
{
    public class EncodingFilter : IFilter<String, Byte[]>, IFilter<Byte[], String>
    {
        private readonly Encoding encoding;

        public EncodingFilter(Encoding encoding)
        {
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public String Execute(Byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return encoding.GetString(input);
        }

        public Byte[] Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return encoding.GetBytes(input);
        }
    }
}