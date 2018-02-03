using System;

namespace SimplePipeline
{
    public sealed class FilterData : IEquatable<FilterData>
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

        public Boolean Equals(FilterData other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Filter, other.Filter) && InputType == other.InputType && OutputType == other.OutputType;
        }

        public static FilterData Create<TFilterInput, TFilterOutput>(IFilter<TFilterInput, TFilterOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new FilterData(filter, typeof(TFilterInput), typeof(TFilterOutput));
        }

        public override Boolean Equals(Object obj)
        {
            return obj is FilterData data && Equals(data);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = Filter != null ? Filter.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (InputType != null ? InputType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OutputType != null ? OutputType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}