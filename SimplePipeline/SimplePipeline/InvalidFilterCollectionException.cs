﻿using System;
using SimplePipeline.Resources;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents an exception that gets thrown when a <see cref="Pipeline{TInput,TOutput}" /> get instantiated with a
    ///     <see cref="FilterCollection" /> that is incompatible with the provided type parameters of the pipeline.
    /// </summary>
    public class InvalidFilterCollectionException : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="InvalidFilterCollectionException" /> instance.
        /// </summary>
        public InvalidFilterCollectionException() : base(ExceptionMessagesResources.InvalidFilterCollectionExceptionMessage) { }
    }
}