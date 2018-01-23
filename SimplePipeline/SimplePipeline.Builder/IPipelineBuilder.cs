using System.Collections.Generic;

namespace SimplePipeline.Builder
{
    public interface IPipelineBuilder : IEnumerable<IFilter>
    {
        IPipelineBuilder Chain(IFilter filter);

        IPipeline Build();
    }

    public interface IPipelineBuilder<in TPipelineInput, out TFilterInput> : IPipelineBuilder
    {
        IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter);

        new IPipeline<TPipelineInput, TFilterInput> Build();
    }
}