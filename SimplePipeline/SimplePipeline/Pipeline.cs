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
        private readonly IEnumerable<FilterData> filters;

        /// <summary>
        ///     Creates a new <see cref="Pipeline{TInput,TOutput}" /> instance.
        /// </summary>
        /// <param name="sequence">The filter sequence to populate this pipeline with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterCollectionException"></exception>
        public Pipeline(FilterSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (!sequence.CanCreatePipeline(typeof(TInput), typeof(TOutput)))
                throw new InvalidFilterCollectionException();
            IEnumerable<FilterData> copyFilterDatas = sequence.ToList();
            filters = copyFilterDatas;
        }

        /// <summary>
        ///     Creates a new <see cref="Pipeline{TInput,TOutput}" /> instance.
        /// </summary>
        /// <param name="filters">The filter collection to populate this pipeline with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterCollectionException"></exception>
        public Pipeline(IEnumerable<FilterData> filters) : this(new FilterSequence(filters)) { }

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
                Output = (TOutput)this.Aggregate<FilterData, Object>(input, (value, filter) => filter.ExecuteFilter(value));
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
            return filters.GetEnumerator();
        }
    }
}