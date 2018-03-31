using System;

namespace SimplePipeline
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IFilter{TInput,TOutput}" /> interface.
    /// </summary>
    public static class Function
    {
        /// <summary>
        ///     Converts a function to a filter.
        /// </summary>
        /// <typeparam name="TInput">The type of the function input.</typeparam>
        /// <typeparam name="TOutput">The type of the function output.</typeparam>
        /// <param name="func">The function to convert to a filter.</param>
        /// <returns>A newly constructed filter that is based on the provided function.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IFilter<TInput, TOutput> ToFilter<TInput, TOutput>(this Func<TInput, TOutput> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return new FuncFilter<TInput, TOutput>(func);
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