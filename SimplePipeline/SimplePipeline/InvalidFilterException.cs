using System;
using SimplePipeline.Resources;

namespace SimplePipeline
{
    public class InvalidFilterException : Exception
    {
        public InvalidFilterException(Type newFilterInputType, Type previousFilterOutputType) : base(String.Format(ExceptionMessagesResources.InvalidFilterExceptionMessage, newFilterInputType, previousFilterOutputType))
        {
                
        }
    }
}