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

    public class Filter<TInput, TOutput> :  IFilter<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> filter;

        public Filter(Func<TInput, TOutput> filter)
        {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public TOutput Execute(TInput input)
        {
            return filter.Invoke(input);
        }

        public Object Execute(Object input)
        {
            return Execute((TInput)input);
        }
    }
}