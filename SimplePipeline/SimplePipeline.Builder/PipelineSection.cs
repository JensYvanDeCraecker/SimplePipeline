using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Builder
{
    public class PipelineSection : IPipelineSection
    {
        private readonly IEnumerable<IFilter> previousFilters;

        public PipelineSection() : this(Enumerable.Empty<IFilter>()) { }

        private PipelineSection(IEnumerable<IFilter> previousFilters)
        {
            this.previousFilters = previousFilters ?? throw new ArgumentNullException(nameof(previousFilters));
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return previousFilters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPipelineSection Chain(IFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new PipelineSection(Concatenate(filter));
        }

        public IPipeline Build()
        {
            return new Pipeline(this);
        }

        private IEnumerable<IFilter> Concatenate(IFilter filter)
        {
            Queue<IFilter> filters = new Queue<IFilter>(this);
            filters.Enqueue(filter);
            return filters;
        }
    }

    public class PipelineSection<TPipelineInput> : PipelineSection<TPipelineInput, TPipelineInput>
    {

    }

    public class PipelineSection<TPipelineInput, TFilterInput> : IPipelineSection<TPipelineInput, TFilterInput>
    {
        private readonly IEnumerable<IFilter> previousFilters;

        protected PipelineSection() : this(Enumerable.Empty<IFilter>()) { }

        private PipelineSection(IEnumerable<IFilter> previousFilters)
        {
            this.previousFilters = previousFilters ?? throw new ArgumentNullException(nameof(previousFilters));
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return previousFilters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPipelineSection Chain(IFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new PipelineSection<TPipelineInput, Object>(Concatenate(filter));
        }

        public IPipeline<TPipelineInput, TFilterInput> Build()
        {
            return new Pipeline<TPipelineInput, TFilterInput>(this);
        }

        public IPipelineSection<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new PipelineSection<TPipelineInput, TFilterOutput>(Concatenate(filter));
        }

        IPipeline IPipelineSection.Build()
        {
            return Build();
        }

        private IEnumerable<IFilter> Concatenate(IFilter filter)
        {
            Queue<IFilter> filters = new Queue<IFilter>(this);
            filters.Enqueue(filter);
            return filters;
        }
    }
}