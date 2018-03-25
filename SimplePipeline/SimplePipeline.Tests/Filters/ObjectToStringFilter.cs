using System;

namespace SimplePipeline.Tests.Filters
{
    public class ObjectToStringFilter : IFilter<Object, String>
    {
        public String Execute(Object input)
        {
            return input.ToString();
        }
    }
}