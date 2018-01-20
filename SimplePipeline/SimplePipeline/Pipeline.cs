using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline : IPipeline
    {
        private readonly IEnumerable<IFilter> filters;

        public Pipeline(IEnumerable<IFilter> filters)
        {
            this.filters = filters ?? throw new ArgumentNullException(nameof(filters));
        }

        public IEnumerator<IFilter> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Object Output { get; private set; }

        public Exception Exception { get; private set; }

        public Boolean Execute(Object input)
        {
            Reset();
            try
            {
                Output = this.Aggregate(input, (value, filter) => filter.Execute(value));
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
            Output = default(Object);
            Exception = default(Exception);
        }
    }
}