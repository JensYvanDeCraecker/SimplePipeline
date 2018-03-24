using System;
using SimplePipeline.Resources;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents an exception that gets thrown when a filter is added to a <see cref="FilterSequence" /> instance and
    ///     that input type of the filter is not assignable from the previous filter output type.
    /// </summary>
    public class InvalidFilterException : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="InvalidFilterException" /> instance.
        /// </summary>
        public InvalidFilterException(Type newFilterInputType, Type previousFilterOutputType) : base(String.Format(ExceptionMessagesResources.InvalidFilterExceptionMessage, newFilterInputType, previousFilterOutputType)) { }
    }
}