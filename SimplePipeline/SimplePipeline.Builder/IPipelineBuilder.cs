using System;
using System.Collections.Generic;

namespace SimplePipeline.Builder
{
    /// <summary>
    ///     Provides methods to chain a filter to a collection of filters and construct a pipeline from these chained filters.
    /// </summary>
    /// <typeparam name="TPipelineInput">The type of the pipeline input.</typeparam>
    /// <typeparam name="TPipelineOutput">The input type of the next chained filter.</typeparam>
    public interface IPipelineBuilder<in TPipelineInput, out TPipelineOutput> : IEnumerable<Object>
    {
        /// <summary>
        ///     Chains a new filter to the collection of filters and returns a new collection of chained filters.
        /// </summary>
        /// <typeparam name="TFilterOutput">The output type of the chained filter.</typeparam>
        /// <param name="filter">The filter to chain.</param>
        /// <returns>A new collection of chained filters.</returns>
        IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TPipelineOutput, TFilterOutput> filter);

        /// <summary>
        ///     Constructs a new pipeline from the collection of chained filters.
        /// </summary>
        /// <returns>A new pipeline that contains the collection of chained filters.</returns>
        IPipeline<TPipelineInput, TPipelineOutput> Build();
    }
}