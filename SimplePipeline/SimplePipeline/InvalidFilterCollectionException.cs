using System;
using SimplePipeline.Resources;

namespace SimplePipeline
{
    public class InvalidFilterCollectionException : Exception
    {
        public InvalidFilterCollectionException() : base(ExceptionMessagesResources.InvalidFilterCollectionException)
        {
        }
    }
}