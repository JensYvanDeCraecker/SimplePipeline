using System;
using System.Collections;
using System.Collections.Generic;
using SimplePipeline;

namespace Pipeline.Testing
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>, IPipelineSection<TInput, TOutput>
    {
        private readonly IEnumerable<IFilter> filters;

        public Pipeline(IEnumerable<IFilter> filters)
        {
            this.filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            throw new NotImplementedException();
        }

        public TOutput Output { get; private set; }

        public Exception Exception { get; private set; }

        public Boolean Execute(Object input)
        {
            return Execute((TInput)input);
        }

        public void Reset()
        {
            Output = default(TOutput);
            Exception = default(Exception);
        }

        public IPipelineSection Chain(IFilter filter)
        {
            return new Pipeline<TInput, Object>(Concatenate(this, filter));
        }

        IPipeline IPipelineSection<TInput, TOutput>.Build()
        {
            return this;
        }

        public IPipelineSection<TInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TOutput, TFilterOutput> filter)
        {
            return new Pipeline<TInput, TFilterOutput>(Concatenate(this, filter));
        }

        IPipeline IPipelineSection.Build()
        {
            return this;
        }

        private static IEnumerable<IFilter> Concatenate(IEnumerable<IFilter> filters, IFilter filter)
        {
            Queue<IFilter> newFilters = new Queue<IFilter>(filters);
            newFilters.Enqueue(filter);
            return newFilters;
        }
    }
}