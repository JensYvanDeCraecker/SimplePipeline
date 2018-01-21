using System;
using System.Collections.Generic;

namespace SimplePipeline
{
    public interface IPipelineSection : IEnumerable<IFilter>
    {
        IPipelineSection Chain(IFilter filter);    

        IPipeline Build();
    }

    public interface IPipelineSection<in TPipelineInput, out TFilterInput> : IPipelineSection
    {
        IPipelineSection<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter);

        new IPipeline Build();
    }
}