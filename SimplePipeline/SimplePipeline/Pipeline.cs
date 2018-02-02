using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly Type filterDefinition = typeof(IFilter<,>);
        private readonly IList<FilterData> filters = new List<FilterData>();

        public Pipeline(IEnumerable<FilterData> filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            foreach (FilterData filter in filters)
                this.filters.Add(filter);
        }

        public Pipeline() { }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TOutput Output { get; private set; }

        public Exception Exception { get; private set; }

        public Boolean IsBeginState
        {
            get
            {
                return Equals(Output, default(TOutput)) && Equals(Exception, default(Exception));
            }
        }

        public Boolean Execute(TInput input)
        {
            Reset();
            try
            {
                Output = (TOutput)this.Aggregate<FilterData, Object>(input, (value, filterData) => filterDefinition.MakeGenericType(filterData.InputType, filterData.OutputType).GetMethod("Execute").Invoke(filterData.Filter, new[] { value }));
                return true;
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
        }

        public void Reset()
        {
            if (IsBeginState)
                return;
            Exception = default(Exception);
            Output = default(TOutput);
        }

        public IEnumerator<FilterData> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        public void Add<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            filters.Add(FilterData.Create(filter));
        }
    }
}