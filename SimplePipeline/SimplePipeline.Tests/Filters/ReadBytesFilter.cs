using System;
using System.IO;

namespace SimplePipeline.Tests.Filters
{
    public class ReadBytesFilter : IFilter<String, Byte[]>
    {
        public Byte[] Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return File.ReadAllBytes(input);
        }
    }
}