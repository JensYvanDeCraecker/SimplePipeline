using System;

namespace SimplePipeline.Builder
{
    public class FilterData
    {
        private FilterData(Object filter, Type inputType, Type outputType)
        {
            Filter = filter;
            InputType = inputType;
            OutputType = outputType;
        }

        public Object Filter { get; }

        public Type InputType { get; }

        public Type OutputType { get; }

        public static FilterData Create<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new FilterData(filter, typeof(TFilterInput), typeof(TFilterOutput));
        }
    }
}