using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePipeline
{
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly MethodInfo baseExecuteFilterMethod = typeof(Pipeline<TInput, TOutput>).GetMethod("ExecuteFilter", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly Type baseFilterType = typeof(IFilter<,>);
        private readonly IList<Object> filters = new List<Object>();

        public Pipeline(IEnumerable<Object> filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            foreach (Object filter in filters)
                this.filters.Add(filter);
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
                Object result = filters.Aggregate<Object, Object>(input, (value, filter) =>
                {
                    Type valueType = value.GetType();
                    if (baseFilterType.MakeGenericType(valueType, typeof(Object)).IsInstanceOfType(filter))
                        return baseExecuteFilterMethod.MakeGenericMethod(valueType).Invoke(this, new[] { filter, value });
                    throw new ArgumentException($"Invalid filter {filter}.");
                });
                Output = result is TOutput output ? output : throw new ArgumentException($"Result is not {typeof(TOutput)}.");
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

        public void Add<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            filters.Add(filter);
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        // ReSharper disable once UnusedMember.Local
        private static Object ExecuteFilter<TFilterInput>(IFilter<TFilterInput, Object> filter, TFilterInput input)
        {
            return filter.Execute(input);
        }
    }
}