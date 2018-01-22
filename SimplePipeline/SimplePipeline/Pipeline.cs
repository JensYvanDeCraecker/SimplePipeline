using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public static class Pipeline
    {
        public static IPipeline Create(IEnumerable<IFilter> filters)
        {
            return Create<Object, Object>(filters);
        }

        public static IPipeline<TInput, TOutput> Create<TInput, TOutput>(IEnumerable<IFilter> filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            return new Pipeline<TInput, TOutput>(filters);
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
                Output = (TOutput)this.Aggregate<IFilter, Object>(input, ExecuteFilter);
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

        private static Object ExecuteFilter(Object input, IFilter filter)
        {
            Type genericFilterType = typeof(IFilter<,>).MakeGenericType(input.GetType(), typeof(Object));
            return genericFilterType.IsInstanceOfType(filter) ? genericFilterType.GetMethod("Execute").Invoke(filter, new[] { input }) : filter.Execute(input);
        }
    }
}