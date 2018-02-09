using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<FilterData> filterDatas;

        public Pipeline(IEnumerable<FilterData> filterDatas)
        {
            if (filterDatas == null)
                throw new ArgumentNullException(nameof(filterDatas));
            IEnumerable<FilterData> copyFilterDatas = filterDatas.ToList();
            if (!ValidateFilterDatas(copyFilterDatas, out Exception exception))
                throw exception;
            this.filterDatas = copyFilterDatas;
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

        private static Boolean ValidateFilterDatas(IEnumerable<FilterData> filterDatas, out Exception exception)
        {
            try
            {
                FilterData first = null;
                FilterData last = null;
                foreach (FilterData filterData in filterDatas)
                {
                    if (filterData == null)
                        throw new ArgumentNullException(nameof(filterData));
                    if (last == null)
                        first = filterData;
                    else if (!filterData.InputType.IsAssignableFrom(last.OutputType))
                        throw new ArgumentException(nameof(filterData));
                    last = filterData;
                }
                if (first != null)
                {
                    if (!first.InputType.IsAssignableFrom(typeof(TInput)))
                        throw new ArgumentException();
                    if (!typeof(TOutput).IsAssignableFrom(last.OutputType))
                        throw new ArgumentException();
                }
                else if (!typeof(TOutput).IsAssignableFrom(typeof(TInput)))
                    throw new ArgumentException();
                exception = default(Exception);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
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