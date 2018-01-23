using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SimplePipeline.Builder
{
    public static class PipelineBuilder
    {
        public static IPipeline<TInput, TOutput> Create<TInput, TOutput>(Func<IPipelineBuilder<TInput, TInput>, IPipelineBuilder<TInput, TOutput>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return func.Invoke(new PipelineBuilder<TInput>()).Build();
        }

        public static IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TPipelineInput, TFilterInput, TFilterOutput>(this IPipelineBuilder<TPipelineInput, TFilterInput> pipelineBuilder, Func<TFilterInput, TFilterOutput> func)
        {
            if (pipelineBuilder == null)
                throw new ArgumentNullException(nameof(pipelineBuilder));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
        return    pipelineBuilder.Chain(func.ToFilter());
        }

        public static IPipelineBuilder Chain(this IPipelineBuilder pipelineBuilder, Func<Object, Object> func)
        {
            if (pipelineBuilder == null)
                throw new ArgumentNullException(nameof(pipelineBuilder));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return pipelineBuilder.Chain(func.ToFilter());
        }
    }

    public class PipelineBuilder<TPipelineInput> : PipelineBuilder<TPipelineInput, TPipelineInput>
    {

    }

    public class PipelineBuilder<TPipelineInput, TFilterInput> : IPipelineBuilder<TPipelineInput, TFilterInput>
    {
        private readonly IEnumerable<IFilter> filters;

        protected PipelineBuilder() : this(Enumerable.Empty<IFilter>())
        {

        }

        public PipelineBuilder(IEnumerable<IFilter> filters)
        {
            this.filters = filters;
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPipelineBuilder Chain(IFilter filter)
        {
            return Create<Object>(filter);
        }

        public IPipeline<TPipelineInput, TFilterInput> Build()
        {
            return new Pipeline<TPipelineInput, TFilterInput>(this);
        }

        public IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            return Create<TFilterOutput>(filter);
        }

        IPipeline IPipelineBuilder.Build()
        {
            return Build();
        }

        private PipelineBuilder<TPipelineInput, TFilterOutput> Create<TFilterOutput>(IFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            Queue<IFilter> newfilters = new Queue<IFilter>(this);
            newfilters.Enqueue(filter);
            return new PipelineBuilder<TPipelineInput, TFilterOutput>(newfilters);
        }
    }
}