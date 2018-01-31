using System;
using System.IO;

namespace SimplePipeline.Example.Filters
{
    public class ReadFileFilter : IFilter<String, String>
    {
        public String Execute(String input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            return File.ReadAllText(input);
        }
    }
}