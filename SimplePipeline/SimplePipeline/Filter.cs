using System;

namespace SimplePipeline
{
    internal class Filter<TFilterInput, TFilterOutput> : IFilter<TFilterInput, TFilterOutput>
    {
        private readonly Func<TFilterInput, TFilterOutput> filter;

        public Filter(Func<TFilterInput, TFilterOutput> filter)
        {
            this.filter = filter;
        }

        public TFilterOutput Execute(TFilterInput input)
        {
            return filter.Invoke(input);
        }
    }
}