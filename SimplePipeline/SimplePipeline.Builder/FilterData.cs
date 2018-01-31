using System;

namespace SimplePipeline.Builder
{
    internal struct FilterData
    {
        public Object Filter { get; set; }

        public Type InputType { get; set; }

        public Type OutputType { get; set; }

        public static FilterData Create<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            return new FilterData()
            {
                Filter = filter,
                InputType = typeof(TInput),
                OutputType = typeof(TOutput)
            };
        }
    }
}