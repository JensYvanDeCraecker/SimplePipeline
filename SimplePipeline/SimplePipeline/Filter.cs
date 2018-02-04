using System;

namespace SimplePipeline
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IFilter{TInput,TOutput}" /> interface.
    /// </summary>
    public static class Filter
    {
        /// <summary>
        ///     Converts a function to a filter.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="func">The function to convert to a filter.</param>
        /// <returns>A newly constructed filter that is based upon the provided function.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IFilter<TInput, TOutput> ToFilter<TInput, TOutput>(this Func<TInput, TOutput> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return new FuncFilter<TInput, TOutput>(func);
        }

        public static FilterData GetData<TInput, TOutput>(this IFilter<TInput, TOutput> filter)
        {
            return FilterData.Create(filter);
        }

        public static IFilter<TInput, TOutput> ToFilter<TInput, TOutput>(this IPipeline<TInput, TOutput> pipeline)
        {
            return ToFilter<TInput, TOutput>(input => pipeline.Execute(input) ? pipeline.Output : throw pipeline.Exception);
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
        }
    }
}