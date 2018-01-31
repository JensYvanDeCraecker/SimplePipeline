using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplePipeline.Builder
{
    internal class FilterEnumerator : IEnumerator<Object>
    {
        private readonly IEnumerator<FilterData> filterDataEnumerator;

        public FilterEnumerator(IEnumerator<FilterData> filterDataEnumerator)
        {
            this.filterDataEnumerator = filterDataEnumerator ?? throw new ArgumentNullException(nameof(filterDataEnumerator));
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

        Object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            filterDataEnumerator.Dispose();
        }
    }
}