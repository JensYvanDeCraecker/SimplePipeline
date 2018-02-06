using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<FilterData> filterDatas;

        public Pipeline(params FilterData[] filterDatas)
        {
            if (filterDatas == null)
                throw new ArgumentNullException(nameof(filterDatas));
            if (!ValidateFilterDatas(filterDatas, out Exception exception))
                throw exception;
            this.filterDatas = filterDatas;
        }

        public Pipeline() : this(new FilterData[0])
        {

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
                Output = (TOutput)this.Aggregate<FilterData, Object>(input, (value, filterData) => filterData.ExecuteFilter(value));
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
            return filterDatas.GetEnumerator();
        }

        private static IEnumerable<FilterData> ValidateFilterDatas(IEnumerable<FilterData> filterDatas)
        {
            Queue<FilterData> validatedFilterDatas = new Queue<FilterData>();
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
                validatedFilterDatas.Enqueue(filterData);
            }
            if (first != null)
            {
                if (!first.InputType.IsAssignableFrom(typeof(TInput)))
                    throw new ArgumentException();
                if (!typeof(TOutput).IsAssignableFrom(last.OutputType))
                    throw new ArgumentException();
            }
            else if (!typeof(TOutput).IsAssignableFrom(typeof(TInput)))
            {
                throw new ArgumentException();
            }
            return validatedFilterDatas;
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
                {
                    throw new ArgumentException();
                }
                exception = default(Exception);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        private class FilterDataCollection : IEnumerable<FilterData>
        {
            private readonly Queue<FilterData> innerCollection = new Queue<FilterData>();

            public FilterDataCollection(IEnumerable<FilterData> filterDatas)
            {
                Add(filterDatas);
            }

            public IEnumerator<FilterData> GetEnumerator()
            {
                return innerCollection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private void Add(IEnumerable<FilterData> filterDatas)
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
                    innerCollection.Enqueue(filterData);
                }
                if (first != null)
                {
                    if (!first.InputType.IsAssignableFrom(typeof(TInput)))
                        throw new ArgumentException();
                    if (!typeof(TOutput).IsAssignableFrom(last.OutputType))
                        throw new ArgumentException();
                }
                else if (!typeof(TOutput).IsAssignableFrom(typeof(TInput)))
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}