using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<FilterData> filterDatas;

        public Pipeline(FilterCollection filterCollection)
        {
            if (filterCollection == null)
                throw new ArgumentNullException(nameof(filterCollection));
            IEnumerable<FilterData> copyFilterDatas = filterCollection.ToList();
            if (!filterCollection.CanCreatePipeline(typeof(TInput), typeof(TOutput)))
                throw new ArgumentException();
            filterDatas = copyFilterDatas;
        }

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
                Output = (TOutput)filterDatas.Aggregate<FilterData, Object>(input, (value, filterData) => filterData.ExecuteFilter(value));
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

        public IEnumerator<Object> GetEnumerator()
        {
            return new Enumerator(filterDatas.GetEnumerator());
        }

        private class Enumerator : IEnumerator<Object>
        {
            private readonly IEnumerator<FilterData> filterDataEnumerator;

            public Enumerator(IEnumerator<FilterData> filterDataEnumerator)
            {
                this.filterDataEnumerator = filterDataEnumerator;
            }

            public Boolean MoveNext()
            {
                return filterDataEnumerator.MoveNext();
            }

            public void Reset()
            {
                filterDataEnumerator.Reset();
            }

            public Object Current
            {
                get
                {
                    return filterDataEnumerator.Current.Filter;
                }
            }

            public void Dispose()
            {
                filterDataEnumerator.Dispose();
            }
        }
    }
}