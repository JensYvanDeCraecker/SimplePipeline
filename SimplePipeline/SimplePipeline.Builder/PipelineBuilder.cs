using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline.Builder
{
    public static class PipelineBuilder
    {
        public static IPipeline<TInput, TOutput> Create<TInput, TOutput>(Func<IPipelineBuilder<TInput, TInput>, IPipelineBuilder<TInput, TOutput>> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return func.Invoke(new PipelineBuilder<TInput>()).Build();
        }

        public static IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TPipelineInput, TFilterInput, TFilterOutput>(this IPipelineBuilder<TPipelineInput, TFilterInput> pipelineBuilder, Func<TFilterInput, TFilterOutput> func)
        {
            if (pipelineBuilder == null)
                throw new ArgumentNullException(nameof(pipelineBuilder));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            return pipelineBuilder.Chain(func.ToFilter());
        }

        public static IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TPipelineInput, TExpectedFilterInput, TFilterInput, TFilterOutput>(this IPipelineBuilder<TPipelineInput, TExpectedFilterInput> pipelineBuilder, Func<TExpectedFilterInput, TFilterInput> inputConverter, IFilter<TFilterInput, TFilterOutput> filter)
        {
            if (pipelineBuilder == null)
                throw new ArgumentNullException(nameof(pipelineBuilder));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (inputConverter == null)
                throw new ArgumentNullException(nameof(inputConverter));
            return pipelineBuilder.Chain(input => filter.Execute(inputConverter.Invoke(input)));
        }

        public static IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TPipelineInput, TFilterInput, TExpectedFilterOutput, TFilterOutput>(this IPipelineBuilder<TPipelineInput, TFilterInput> pipelineBuilder, IFilter<TFilterInput, TExpectedFilterOutput> filter, Func<TExpectedFilterOutput, TFilterOutput> outputConverter)
        {
            if (pipelineBuilder == null)
                throw new ArgumentNullException(nameof(pipelineBuilder));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (outputConverter == null)
                throw new ArgumentNullException(nameof(outputConverter));
            return pipelineBuilder.Chain(input => outputConverter.Invoke(filter.Execute(input)));
        }

        public static IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TPipelineInput, TExpectedFilterInput, TFilterInput, TExpectedFilterOutput, TFilterOutput>(this IPipelineBuilder<TPipelineInput, TExpectedFilterInput> pipelineBuilder, Func<TExpectedFilterInput, TFilterInput> inputConverter, IFilter<TFilterInput, TExpectedFilterOutput> filter, Func<TExpectedFilterOutput, TFilterOutput> outputConverter)
        {
            if (pipelineBuilder == null)
                throw new ArgumentNullException(nameof(pipelineBuilder));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (outputConverter == null)
                throw new ArgumentNullException(nameof(outputConverter));
            if (inputConverter == null)
                throw new ArgumentNullException(nameof(inputConverter));
            return pipelineBuilder.Chain(input => outputConverter.Invoke(filter.Execute(inputConverter.Invoke(input))));
        }
    }

    internal class PipelineBuilder<TPipelineInput> : PipelineBuilder<TPipelineInput, TPipelineInput> { }

    internal class PipelineBuilder<TPipelineInput, TPipelineOutput> : IPipelineBuilder<TPipelineInput, TPipelineOutput>
    {
        private readonly IEnumerable<FilterData> filterDatas;

        protected PipelineBuilder() : this(Enumerable.Empty<FilterData>()) { }

        private PipelineBuilder(IEnumerable<FilterData> filterDatas)
        {
            this.filterDatas = filterDatas ?? throw new ArgumentNullException(nameof(filterDatas));
        }

        public IPipeline<TPipelineInput, TPipelineOutput> Build()
        {
            return new Pipeline(this);
        }

        public IPipelineBuilder<TPipelineInput, TFilterOutput> Chain<TFilterOutput>(IFilter<TPipelineOutput, TFilterOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            Queue<FilterData> newfilterDatas = new Queue<FilterData>(this);
            newfilterDatas.Enqueue(FilterData.Create(filter));
            return new PipelineBuilder<TPipelineInput, TFilterOutput>(newfilterDatas);
        }

        public IEnumerator<FilterData> GetEnumerator()
        {
            return filterDatas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Pipeline : IPipeline<TPipelineInput, TPipelineOutput>
        {
            private readonly Type baseFilterType = typeof(IFilter<,>);
            private readonly IEnumerable<FilterData> filterDatas;

            public Pipeline(IEnumerable<FilterData> filterDatas)
            {
                this.filterDatas = filterDatas;
            }

            public IEnumerator<Object> GetEnumerator()
            {
                return new FilterEnumerator(filterDatas.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public TPipelineOutput Output { get; private set; }

            public Exception Exception { get; private set; }

            public Boolean IsBeginState
            {
                get
                {
                    return Equals(Output, default(TPipelineOutput)) && Equals(Exception, default(Exception));
                }
            }

            public Boolean Execute(TPipelineInput input)
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                Reset();
                try
                {
                    Output = (TPipelineOutput)filterDatas.Aggregate<FilterData, Object>(input, (value, filterData) => baseFilterType.MakeGenericType(filterData.InputType, filterData.OutputType).GetMethod("Execute").Invoke(filterData.Filter, new[] { value }));
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
                Output = default(TPipelineOutput);
            }

            private class FilterEnumerator : IEnumerator<Object>
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
    }
}