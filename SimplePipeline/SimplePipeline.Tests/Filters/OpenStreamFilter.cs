using System;
using System.IO;

namespace SimplePipeline.Tests.Filters
{
    public class OpenStreamFilter : IFilter<String, Stream>
    {
        public Stream Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return File.OpenRead(input);
        }
    }
}