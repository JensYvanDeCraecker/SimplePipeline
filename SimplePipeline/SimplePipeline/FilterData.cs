using System;
using System.Reflection;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents a filter in a non-generic environment.
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
        ///     Gets the filter that this data represents.
        /// </summary>
        public Object Filter { get; }

        /// <summary>
        ///     Gets the input type that this data represents.
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        ///     Gets the output type that this data represents.
        /// </summary>
        public Type OutputType { get; }

        /// <summary>
        ///     Returns the <see cref="IFilter{TInput,TOutput}" /> type that this information is based on.
        /// </summary>
        public Type FilterType { get; }

        /// <summary>
        ///     Checks if the provided data is equal to this data.
        /// </summary>
        /// <param name="other">The data to check for equality.</param>
        /// <returns>Returns a boolean that indicates if this data equal to the provided data.</returns>
        public Boolean Equals(FilterData other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Filter, other.Filter) && InputType == other.InputType && OutputType == other.OutputType;
        }

        /// <summary>
        ///     Creates a non-generic filter representation.
        /// </summary>
        /// <typeparam name="TInput">The type of the filter input.</typeparam>
        /// <typeparam name="TOutput">The type of the filter output.</typeparam>
        /// <param name="filter">The filter to create a non-generic representation from.</param>
        /// <returns>The non-generic representation of the provided filter.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static FilterData Create<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new FilterData(filter, typeof(TInput), typeof(TOutput));
        }

        /// <summary>
        ///     Checks if the provided object is equal to this data.
        /// </summary>
        /// <param name="obj">The object to check for equality.</param>
        /// <returns>Returns a boolean that indicates if this data equal to the provided object.</returns>
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
        ///     Executes the filter of this non-generic representation.
        /// </summary>
        /// <param name="input">The input for the filter to process.</param>
        /// <returns>The processed output of the filter.</returns>
        public Object ExecuteFilter(Object input)
        {
            try
            {
                return executeFilter.Invoke(Filter, new[] { input });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}