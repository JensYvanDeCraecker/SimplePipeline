using System;

namespace SimplePipeline
{
    public static class Filter
    {
        public static IFilter Create(Func<Object, Object> filter)
        {
            return Create<Object, Object>(filter);
        }

        public static IFilter<TInput, TOutput> Create<TInput, TOutput>(Func<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new Filter<TInput, TOutput>(filter);
        }
    }

    public class Filter<TInput, TOutput> : IFilter<TInput, TOutput>
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