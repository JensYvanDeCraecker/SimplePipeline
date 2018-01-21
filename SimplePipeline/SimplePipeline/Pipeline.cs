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

    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<IFilter> filters;

        public Pipeline(IEnumerable<IFilter> filters)
        {
            this.filters = filters;
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
                Output = (TOutput)this.Aggregate<IFilter, Object>(input, (value, filter) => ExecuteFilter(filter, value));
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

        private static Object ExecuteFilter(IFilter filter, Object input)
        {
            Type genericFilterType = typeof(IFilter<,>).MakeGenericType(input.GetType(), typeof(Object));
            return genericFilterType.IsInstanceOfType(filter) ? genericFilterType.GetMethod("Execute").Invoke(filter, new[] { input }) : filter.Execute(input);
        }
    }
}