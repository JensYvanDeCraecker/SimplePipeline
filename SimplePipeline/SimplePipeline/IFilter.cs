using System;

namespace SimplePipeline
{
    /// <summary>
    ///     Provides a method that can process a given input and returns an output.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        ///     Processes the input and returns the processed output.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <returns>The processed output.</returns>
        Object Execute(Object input);
    }

    /// <summary>
    ///     Provides a method that can process a given generic input and returns a generic output.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IFilter<in TInput, out TOutput> : IFilter
    {
        /// <summary>
        ///     Processes the input and returns the processed output.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <returns>The processed output.</returns>
        TOutput Execute(TInput input);
    }
}