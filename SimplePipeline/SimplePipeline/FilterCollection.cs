using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents a sequence of filters that the output of previous filter matches the input of the following filter.
    /// </summary>
    public sealed class FilterCollection : IReadOnlyCollection<FilterData>
    {
        private readonly Queue<FilterData> innerCollection = new Queue<FilterData>();
        private FilterData first;
        private FilterData last;

        /// <summary>
        /// Creates a new <see cref="FilterCollection"/> instance.
        /// </summary>
        public FilterCollection()
        {
                
        }

        /// <summary>
        /// Create a new <see cref="FilterCollection"/> instance.
        /// </summary>
        /// <param name="filterDatas">The collection of data to add to this sequence.</param>
        // ReSharper disable once UnusedMember.Global
        public FilterCollection(IEnumerable<FilterData> filterDatas)
        {
            if (filterDatas == null)
                throw new ArgumentNullException(nameof(filterDatas));
            Load(filterDatas);

        }

        private void Load(IEnumerable<FilterData> filterDatas)
        {
            foreach (FilterData filterData in filterDatas)
                Add(filterData);
        }

        /// <summary>
        ///     Gets the input type of the first filter in this sequence.
        /// </summary>
        public Type InputType
        {
            get
            {
                return first?.InputType;
            }
        }

        /// <summary>
        ///     Get the output type of the last filter in this sequence.
        /// </summary>
        public Type OutputType
        {
            get
            {
                return last?.OutputType;
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<FilterData> GetEnumerator()
        {
            return innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Gets the number of elements in the collection.
        /// </summary>
        public Int32 Count
        {
            get
            {
                return innerCollection.Count;
            }
        }

        /// <summary>
        ///     Adds a filter to the end of the sequence.
        /// </summary>
        /// <typeparam name="TInput">The type of the filter input.</typeparam>
        /// <typeparam name="TOutput">The type of the filter output.</typeparam>
        /// <param name="filter">The filter to add to the end of the sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterException"></exception>
        public void Add<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            Add(FilterData.Create(filter));
        }

        /// <summary>
        ///     Adds filter information to the end of the sequence.
        /// </summary>
        /// <param name="filterData">The filter information to add to the end of the sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterException"></exception>
        public void Add(FilterData filterData)
        {
            if (filterData == null)
                throw new ArgumentNullException(nameof(filterData));
            if (first == null)
                first = filterData;
            else if (!filterData.InputType.IsAssignableFrom(last.OutputType))
                throw new InvalidFilterException(filterData.InputType, last.OutputType);
            last = filterData;
            innerCollection.Enqueue(filterData);
        }

        /// <summary>
        ///     Checks whether this sequence of filters can be used to create a pipeline of the provided input and output type.
        /// </summary>
        /// <param name="pipelineInputType">The type of the pipeline input to check if it is compatible with this sequence.</param>
        /// <param name="pipelineOutputType">The type of the pipeline output to check if it is compatible with this sequence.</param>
        /// <returns>A value indicating if this sequence is compatible.</returns>
        public Boolean CanCreatePipeline(Type pipelineInputType, Type pipelineOutputType)
        {
            return pipelineInputType != null && pipelineOutputType != null && (Count > 0 ? InputType.IsAssignableFrom(pipelineInputType) && pipelineOutputType.IsAssignableFrom(OutputType) : pipelineOutputType.IsAssignableFrom(pipelineInputType));
        }
    }
}