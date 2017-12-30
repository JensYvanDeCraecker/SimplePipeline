using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SimplePipeline
{
    public static class Pipeline
    {
        public static IPipeline<TPipelineInput, TPipelineOutput> Create<TPipelineInput, TPipelineOutput>(Func<PipelineBuilder<TPipelineInput>, PipelineBuilder<TPipelineInput,TPipelineOutput>> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.Invoke(new PipelineBuilder<TPipelineInput>()).Build();
        }

        public static IPipeline<TPipelineInput, TPipelineOutput> Create<TPipelineInput, TPipelineOutput>(IFilter<TPipelineInput, TPipelineOutput> filter)
        {
            return Create<TPipelineInput, TPipelineOutput>(builder => builder.Chain(filter));
        }

        public static IPipeline<TPipelineInput, TPipelineOutput> Create<TPipelineInput, TPipelineOutput>(Func<TPipelineInput, TPipelineOutput> filter)
        {
            return Create(Filter.Create(filter));
        }
    }
    internal class Pipeline<TPipelineInput, TPipelineOutput> : IPipeline<TPipelineInput, TPipelineOutput>
    {
        private readonly IEnumerable<Object> filters;

        public Pipeline(IEnumerable<Object> filters)
        {
            List<Object> filterList = filters.ToList();
            this.filters = filterList;
            Count = filterList.Count;
        }

        public TPipelineOutput Execute(TPipelineInput input)
        {
            return (TPipelineOutput)filters.Aggregate<Object, Object>(input, (current, filter) => typeof(IFilter<,>).MakeGenericType(current.GetType(), typeof(Object)).GetMethod("Execute").Invoke(filter, new[] { current }));
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return filters.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Int32 Count { get; }
    }
}