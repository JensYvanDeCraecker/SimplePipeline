using System;

namespace SimplePipeline.Generic
{
    public interface IPipeline<in TInput, out TOutput> : IPipeline
    {
        new TOutput Output { get; }

        Boolean Execute(TInput input);
    }
}