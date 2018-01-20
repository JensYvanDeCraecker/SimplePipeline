using System;

namespace SimplePipeline.Generic
{
    public class Filter<TInput, TOutput> : IFilter<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> filter;

        public Filter(Func<TInput, TOutput> filter)
        {
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public TOutput Execute(TInput input)
        {
            return filter.Invoke(input);
        }

        public Object Execute(Object input)
        {
            return Execute((TInput)input);
        }
    }
}