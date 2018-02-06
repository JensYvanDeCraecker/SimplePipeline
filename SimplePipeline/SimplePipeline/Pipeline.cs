using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly FilterDataCollection filterDatas = new FilterDataCollection();

        public Pipeline(params FilterData[] filterDatas)
        {
            if (filterDatas == null)
                throw new ArgumentNullException(nameof(filterDatas));
            Type pipelineInputType = typeof(TInput);
            Type pipelineOutputType = typeof(TOutput);
            foreach (FilterData filterData in filterDatas)
                this.filterDatas.Add(filterData);
            if (!this.filterDatas.First.InputType.IsAssignableFrom(pipelineInputType))
                throw new ArgumentException();
            if (!pipelineOutputType.IsAssignableFrom(this.filterDatas.Last.OutputType))
                throw new ArgumentException();
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

        private class FilterDataCollection : IEnumerable<FilterData>
        {
            private readonly Queue<FilterData> innerCollection = new Queue<FilterData>();

            public FilterData First { get; private set; }

            public FilterData Last { get; private set; }

            public IEnumerator<FilterData> GetEnumerator()
            {
                return innerCollection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(FilterData filterData)
            {
                if (filterData == null)
                    throw new ArgumentNullException(nameof(filterData));
                if (Last == null)
                    First = filterData;
                else if (!filterData.InputType.IsAssignableFrom(Last.OutputType))
                    throw new ArgumentException(nameof(filterData));
                Last = filterData;
                innerCollection.Enqueue(filterData);
            }
        }
    }
}