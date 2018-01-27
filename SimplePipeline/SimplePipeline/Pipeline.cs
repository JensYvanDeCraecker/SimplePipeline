using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly MethodInfo executeGenericFilter = typeof(Pipeline<TInput, TOutput>).GetMethod("ExecuteGenericfilter", BindingFlags.Static | BindingFlags.NonPublic);
        private readonly IEnumerable<IFilter> filters;
        private readonly Type genericFilterType = typeof(IFilter<,>);

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
            if (IsBeginState)
                return;
            Output = default(TOutput);
            Exception = default(Exception);
        }

        public Boolean IsBeginState
        {
            get
            {
                return Equals(Output, default(TOutput)) && Exception == default(Exception);
            }
        }

        private Object ExecuteFilter(Object input, IFilter filter)
        {
            Type inputType = input.GetType();
            return genericFilterType.MakeGenericType(inputType, typeof(Object)).IsInstanceOfType(filter) ? executeGenericFilter.MakeGenericMethod(inputType).Invoke(filter, new[] { input, filter }) : filter.Execute(input);
        }

        // ReSharper disable once UnusedMember.Local
        private static Object ExecuteGenericfilter<TFilterInput>(TFilterInput input, IFilter<TFilterInput, Object> filter)
        {
            return filter.Execute(input);
        }
    }
}