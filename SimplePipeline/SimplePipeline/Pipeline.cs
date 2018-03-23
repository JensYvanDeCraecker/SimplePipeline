using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents a concrete implementation of the <see cref="IPipeline{TInput,TOutput}" /> interface.
    /// </summary>
    /// <typeparam name="TInput">The type of the pipeline input.</typeparam>
    /// <typeparam name="TOutput">The type of the pipeline output.</typeparam>
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<FilterData> filterDatas;

        /// <summary>
        ///     Creates a new <see cref="Pipeline{TInput,TOutput}" /> instance.
        /// </summary>
        /// <param name="filterCollection">The filter sequence to populate this pipeline with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterCollectionException"></exception>
        public Pipeline(FilterCollection filterCollection)
        {
            if (filterCollection == null)
                throw new ArgumentNullException(nameof(filterCollection));
            if (!filterCollection.CanCreatePipeline(typeof(TInput), typeof(TOutput)))
                throw new InvalidFilterCollectionException();
            IEnumerable<FilterData> copyFilterDatas = filterCollection.ToList();
            filterDatas = copyFilterDatas;
        }

        /// <summary>
        ///     Creates a new <see cref="Pipeline{TInput,TOutput}" /> instance.
        /// </summary>
        /// <param name="filterDatas">The filter collection to populate this pipeline with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterCollectionException"></exception>
        public Pipeline(IEnumerable<FilterData> filterDatas) : this(new FilterCollection(filterDatas)) { }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Gets the output of a processed input, if successful. If not, the default value is returned.
        /// </summary>
        public TOutput Output { get; private set; }

        /// <summary>
        ///     Gets the exception of a processed input, if unsuccessful. If successful, the default value is returned.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///     Gets the state of the pipeline.
        /// </summary>
        public Boolean IsBeginState
        {
            get
            {
                return Equals(Output, default(TOutput)) && Equals(Exception, default(Exception));
            }
        }

        /// <summary>
        ///     Processes the input in a collection of filters and returns a boolean that determines if the processing was
        ///     successful.
        /// </summary>
        /// <param name="input">The input to process in a collection of filters.</param>
        /// <returns>True if the processing was successful. If not, false is returned.</returns>
        public Boolean Execute(TInput input)
        {
            Reset();
            try
            {
                Output = (TOutput)filterDatas.Aggregate<FilterData, Object>(input, (value, filterData) => filterData.ExecuteFilter(value));
                return true;
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
        }

        /// <summary>
        ///     Resets the pipeline to a state that is similar to a pipeline that has not yet processed any inputs.
        /// </summary>
        public void Reset()
        {
            if (IsBeginState)
                return;
            Exception = default(Exception);
            Output = default(TOutput);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Object> GetEnumerator()
        {
            return filterDatas.Select(data => data.Filter).GetEnumerator();
        }
    }
}