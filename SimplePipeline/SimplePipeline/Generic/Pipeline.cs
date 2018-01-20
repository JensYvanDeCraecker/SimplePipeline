using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Generic
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
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

        Object IPipeline.Output
        {
            get
            {
                return Output;
            }
        }

        public Boolean Execute(TInput input)
        {
            Reset();
            try
            {
                Output = (TOutput)this.Aggregate<IFilter, Object>(input, (value, filter) => filter.Execute(value));
                return true;
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
        }

        public TOutput Output { get; private set; }

        public Exception Exception { get; private set; }

        public Boolean Execute(Object input)
        {
            return Execute((TInput)input);
        }

        public void Reset()
        {
            Output = default(TOutput);
            Exception = default(Exception);
        }
    }
}