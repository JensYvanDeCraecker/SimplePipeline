using System;

namespace SimplePipeline
{
    public static class Filter
    {

        public static IFilter<TInput, TOutput> ToFilter<TInput, TOutput>(this Func<TInput, TOutput> filter)
        {
            return new Filter<TInput, TOutput>(filter);
        }

        public static IFilter ToFilter(this Func<Object, Object> filter)
        {
            return new Filter<Object, Object>(filter);
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