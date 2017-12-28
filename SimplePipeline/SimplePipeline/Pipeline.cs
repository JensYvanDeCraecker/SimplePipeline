using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
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
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Int32 Count { get; }
    }
}