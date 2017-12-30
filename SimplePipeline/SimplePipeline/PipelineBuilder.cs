using System;
using System.Collections.Generic;

namespace SimplePipeline
{
    public class PipelineBuilder<TPipelineInput> : PipelineBuilder<TPipelineInput, TPipelineInput> // This class enforces that the first filter input is the same type as the pipeline input
    {

    }

    public class PipelineBuilder<TPipelineInput, TFilterInput>
    {
        private readonly Queue<Object> previousFilters;

        private PipelineBuilder(Queue<Object> previousFilters)
        {
            this.previousFilters = previousFilters;
        }

        protected PipelineBuilder() : this(new Queue<Object>())
        {
        }

        public PipelineBuilder<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            previousFilters.Enqueue(filter);
            return new PipelineBuilder<TPipelineInput, TFilterOutput>(previousFilters);
        }

        public PipelineBuilder<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(Func<TFilterInput, TFilterOutput> filter)
        {
            return Chain(Filter.Create(filter));
        }

        public IPipeline<TPipelineInput, TFilterInput> Build()
        {
            return new Pipeline<TPipelineInput, TFilterInput>(previousFilters);
        }
    }
}