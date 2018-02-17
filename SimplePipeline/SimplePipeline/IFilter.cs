namespace SimplePipeline
{
    /// <summary>
    ///     Provides a method that can process a given input and returns an output.
    /// </summary>
    /// <typeparam name="TInput">The type of the filter input.</typeparam>
    /// <typeparam name="TOutput">The type of the filter output.</typeparam>
    public interface IFilter<in TInput, out TOutput>
    {
        /// <summary>
        ///     Processes the input and returns the processed output.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <returns>The processed output.</returns>
        TOutput Execute(TInput input);
    }
}