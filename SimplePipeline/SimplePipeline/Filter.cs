using System;

namespace SimplePipeline
{
    public class Filter : IFilter
    {
        private readonly Func<Object, Object> filter;

        public Filter(Func<Object, Object> filter)
        {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public Object Execute(Object input)
        {
            return filter.Invoke(input);
        }
    }
}