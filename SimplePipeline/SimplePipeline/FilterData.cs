using System;
using System.Reflection;

namespace SimplePipeline
{
    /// <summary>
    ///     Contains information about a filter.
    /// </summary>
    public sealed class FilterData : IEquatable<FilterData>
    {
        private readonly MethodInfo executeFilter;

        private FilterData(Object filter, Type inputType, Type outputType)
        {
            Filter = filter;
            InputType = inputType;
            OutputType = outputType;
            FilterType = typeof(IFilter<,>).MakeGenericType(inputType, outputType);
            executeFilter = FilterType.GetMethod("Execute");
        }

        /// <summary>
        ///     Returns the filter that this information is based on.
        /// </summary>
        public Object Filter { get; }

        /// <summary>
        ///     Returns the type of the input of the filter that this information is based on.
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        ///     Returns the type of the output of the filter that this information is based on.
        /// </summary>
        public Type OutputType { get; }

        /// <summary>
        ///     Returns the <see cref="IFilter{TInput,TOutput}" /> type that this information is based on.
        /// </summary>
        public Type FilterType { get; }

        /// <summary>
        ///     Checks if the provided information is equal to this information.
        /// </summary>
        /// <param name="other">The information to check for equality.</param>
        /// <returns>Returns a boolean that indicates if this information equal to the provided information.</returns>
        public Boolean Equals(FilterData other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Filter, other.Filter) && InputType == other.InputType && OutputType == other.OutputType;
        }

        /// <summary>
        ///     Creates information from the provided filter.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="filter">The filter to create the information from.</param>
        /// <returns>The information about the provided filter.</returns>
        public static FilterData Create<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new FilterData(filter, typeof(TInput), typeof(TOutput));
        }

        /// <summary>
        ///     Checks if the provided object is equal to this information.
        /// </summary>
        /// <param name="obj">The object to check for equality.</param>
        /// <returns>Returns a boolean that indicates if this information equal to the provided object.</returns>
        public override Boolean Equals(Object obj)
        {
            return obj is FilterData data && Equals(data);
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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

        /// <summary>
        ///     Execute the filter that this information is based on.
        /// </summary>
        /// <param name="input">The input for the filterto process.</param>
        /// <returns>The processed output of the filter.</returns>
        public Object ExecuteFilter(Object input)
        {
            return executeFilter.Invoke(Filter, new[] { input });
        }
    }
}