using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplePipeline
{
    public sealed class FilterDataCollection : IEnumerable<FilterData>
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