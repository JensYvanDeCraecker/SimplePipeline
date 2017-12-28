using System;
using System.Collections.Generic;

namespace SimplePipeline
{
    public interface IPipeline<in TPipelineInput, out TPipelineOutput> : IFilter<TPipelineInput, TPipelineOutput>, IEnumerable<Object>
    {
        Int32 Count { get; }
    }
}