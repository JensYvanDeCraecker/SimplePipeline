using System;
using System.Text;

namespace SimplePipeline.Tests.Filters
{
    public class GetBytesFilter : IFilter<String, Byte[]>
    {
        public Byte[] Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return Encoding.Unicode.GetBytes(input);
        }
    }
}