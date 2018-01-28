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

        object IPipeline.Output
        {
            get
            {
                return Output;
            }
        }

        public bool Execute(TInput input)
        {
            Reset();
            try
            {
                Output = (TOutput) this.Aggregate<IFilter, object>(input, ExecuteFilter);
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

        public bool Execute(object input)
        {
            return Execute((TInput) input);
        }

        public void Reset()
        {
            if (IsBeginState)
                return;
            Output = default(TOutput);
            Exception = default(Exception);
        }

        public bool IsBeginState
        {
            get
            {
                return Equals(Output, default(TOutput)) && Exception == default(Exception);
            }
        }

        private object ExecuteFilter(object input, IFilter filter)
        {
            Type inputType = input.GetType();
            return genericFilterType.MakeGenericType(inputType, typeof(object)).IsInstanceOfType(filter) ? executeGenericFilter.MakeGenericMethod(inputType).Invoke(filter, new[] {input, filter}) : filter.Execute(input);
        }

        // ReSharper disable once UnusedMember.Local
        private static object ExecuteGenericfilter<TFilterInput>(TFilterInput input, IFilter<TFilterInput, object> filter)
        {
            return filter.Execute(input);
        }
    }
}