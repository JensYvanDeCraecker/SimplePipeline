using System;
using System.Reflection;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents a filter in a non-generic environment.
    /// </summary>
    public sealed class FilterData : IEquatable<FilterData>, IFilter<Object, Object>
    {
        private readonly MethodInfo executeFilter;

        private FilterData(Object filter, Type inputType, Type outputType)
        {
            Filter = filter;
            InputType = inputType;
            OutputType = outputType;
            executeFilter = typeof(FilterData).GetMethod(nameof(ExecuteFilter), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(InputType, OutputType);
        }

        /// <summary>
        ///     Gets the filter instance.
        /// </summary>
        public Object Filter { get; }

        /// <summary>
        ///     Gets the input type of the filter.
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        ///     Gets the output type of the filter.
        /// </summary>
        public Type OutputType { get; }

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
        ///     Creates a non-generic filter.
        /// </summary>
        /// <typeparam name="TInput">The type of the filter input.</typeparam>
        /// <typeparam name="TOutput">The type of the filter output.</typeparam>
        /// <param name="filter">The filter to create a non-generic filter from.</param>
        /// <returns>The non-generic filter of the provided filter.</returns>
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
                Int32 hashCode = Filter.GetHashCode();
                hashCode = (hashCode * 397) ^ InputType.GetHashCode();
                hashCode = (hashCode * 397) ^ OutputType.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Executes the filter of this non-generic filter.
        /// </summary>
        /// <param name="input">The input for the filter to process.</param>
        /// <returns>The processed output of the filter.</returns>
        public Object Execute(Object input)
        {
            try
            {
                return executeFilter.Invoke(null, new[] { Filter, input });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public static Boolean operator ==(FilterData first, FilterData second)
        {
            return Equals(first, second);
        }

        public static Boolean operator !=(FilterData first, FilterData second)
        {
            return !(first == second);
        }

        private static TOutput ExecuteFilter<TInput, TOutput>(IFilter<TInput, TOutput> filter, TInput input)
        {
            return filter.Execute(input);
        }
    }
}