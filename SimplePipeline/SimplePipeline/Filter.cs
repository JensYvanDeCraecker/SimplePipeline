using System;

namespace SimplePipeline
{
    /// <summary>
    /// Provides extension methods for <see cref="IFilter"/> and <see cref="IFilter{TInput,TOutput}"/> interfaces.
    /// </summary>
    public static class Filter
    {
        public static IFilter<TInput, TOutput> ToFilter<TInput, TOutput>(this Func<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new FuncFilter<TInput, TOutput>(filter);
        }

        public static IFilter ToFilter(this Func<Object, Object> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return ToFilter<Object, Object>(filter);
        }

        private class FuncFilter<TInput, TOutput> : IFilter<TInput, TOutput>
        {
            private readonly Func<TInput, TOutput> filter;

            public FuncFilter(Func<TInput, TOutput> filter)
            {
                this.filter = filter;
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
}