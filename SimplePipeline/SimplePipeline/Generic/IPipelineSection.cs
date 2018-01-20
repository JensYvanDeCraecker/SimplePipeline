using System;

namespace SimplePipeline.Generic
{
    public interface IPipelineSection<in TPipelineInput, out TFilterInput> : IPipelineSection
    {
        IPipelineSection<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter);

        IPipelineSection<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(Func<TFilterInput, TFilterOutput> filter);

        new IPipeline Build();
    }
}