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

        /// <inheritdoc />
        public TOutput Output { get; private set; }

        /// <inheritdoc />
        public Exception Exception { get; private set; }

        /// <inheritdoc />
        public Boolean IsBeginState
        {
            get
            {
                return Equals(Output, default(TOutput)) && Equals(Exception, default(Exception));
            }
        }

        /// <inheritdoc />
        public Boolean Execute(TInput input)
        {
            Reset();
            try
            {
                Output = (TOutput)this.Aggregate<FilterData, Object>(input, (value, filterData) => filterData.ExecuteFilter(value));
                return true;
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            if (IsBeginState)
                return;
            Exception = default(Exception);
            Output = default(TOutput);
        }

        /// <inheritdoc />
        public IEnumerator<FilterData> GetEnumerator()
        {
            return filterDatas.GetEnumerator();
        }
    }
}