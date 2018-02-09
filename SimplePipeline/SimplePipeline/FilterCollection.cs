using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplePipeline
{
    public sealed class FilterCollection : IReadOnlyCollection<FilterData>
    {
        private readonly Queue<FilterData> innerCollection = new Queue<FilterData>();
        private FilterData first;
        private FilterData last;

        public Type InputType
        {
            get
            {
                return first?.InputType;
            }
        }

        public Type OutputType
        {
            get
            {
                return last?.OutputType;
            }
        }

        public void Add<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            FilterData filterData = FilterData.Create(filter);
            if (first == null)
                first = filterData;
            else if (!filterData.InputType.IsAssignableFrom(last.OutputType))
                throw new ArgumentException(nameof(filterData));        
            last = filterData;
            innerCollection.Enqueue(filterData);
        }

        public Boolean CanCreatePipeline(Type pipelineInputType, Type pipelineOutputType)
        {
            if (pipelineInputType == null)
                throw new ArgumentNullException(nameof(pipelineInputType));
            if (pipelineOutputType == null)
                throw new ArgumentNullException(nameof(pipelineOutputType));
            if (Count > 0)
                return InputType.IsAssignableFrom(pipelineInputType) && pipelineOutputType.IsAssignableFrom(OutputType);
            return pipelineOutputType.IsAssignableFrom(pipelineInputType);
        }

        public IEnumerator<FilterData> GetEnumerator()
        {
            return innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Int32 Count
        {
            get
            {
                return innerCollection.Count;
            }
        }
    }
}