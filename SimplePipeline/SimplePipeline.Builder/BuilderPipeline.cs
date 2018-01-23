using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Builder
{
    public static class BuilderPipeline
    {
        public static BuilderPipeline<TPipelineInput, TFilterOutput> Chain<TPipelineInput, TFilterOutput>(IFilter<TPipelineInput,TFilterOutput> filter)
        {
            return new BuilderPipeline<TPipelineInput>().Chain(filter);
        }
    }

    public class BuilderPipeline<TInput> : BuilderPipeline<TInput, TInput> { }

    public class BuilderPipeline<TInput, TOutput> : IPipelineSection<TInput, TOutput>, IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<IFilter> filters;

        protected BuilderPipeline() : this(Enumerable.Empty<IFilter>()) { }

        private BuilderPipeline(IEnumerable<IFilter> filters)
        {
            this.filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        Object IPipeline.Output
        {
            get
            {
                return Output;
            }
        }

        public Boolean Execute(TInput input)
        {
            Reset();
            try
            {
                Output = (TOutput)this.Aggregate<IFilter, Object>(input, ExecuteFilter);
                return true;
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
        }

        public TOutput Output { get; private set; }

        public Exception Exception { get; private set; }

        Boolean IPipeline.Execute(Object input)
        {
            return Execute((TInput)input);
        }

        public void Reset()
        {
            Output = default(TOutput);
            Exception = default(Exception);
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IPipelineSection IPipelineSection.Chain(IFilter filter)
        {
            return new BuilderPipeline<TInput, Object>(Concatenate(filter));
        }

        IPipeline<TInput, TOutput> IPipelineSection<TInput, TOutput>.Build()
        {
            return new BuilderPipeline<TInput, TOutput>(this);
        }

        IPipelineSection<TInput, TFilterOutput> IPipelineSection<TInput, TOutput>.Chain<TFilterOutput>(IFilter<TOutput, TFilterOutput> filter)
        {
            return Chain(filter);
        }

        IPipeline IPipelineSection.Build()
        {
            return ((IPipelineSection<TInput, TOutput>)this).Build();
        }

        public BuilderPipeline<TInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TOutput, TFilterOutput> filter)
        {
            return new BuilderPipeline<TInput, TFilterOutput>(Concatenate(filter));
        }

        private IEnumerable<IFilter> Concatenate(IFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            Queue<IFilter> newfilters = new Queue<IFilter>(this);
            newfilters.Enqueue(filter);
            return newfilters;
        }

        private static Object ExecuteFilter(Object input, IFilter filter)
        {
            Type genericFilterType = typeof(IFilter<,>).MakeGenericType(input.GetType(), typeof(Object));
            return genericFilterType.IsInstanceOfType(filter) ? genericFilterType.GetMethod("Execute").Invoke(filter, new[] { input }) : filter.Execute(input);
        }
    }
}