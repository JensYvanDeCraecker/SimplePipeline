using System;

namespace SimplePipeline
{
    public static class Filter
    {
        public static IFilter<TFilterInput, TFilterOutput> Create<TFilterInput, TFilterOutput>(Func<TFilterInput, TFilterOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new Filter<TFilterInput, TFilterOutput>(filter);
        }
    }

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