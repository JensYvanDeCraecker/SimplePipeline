using System;
using System.Collections.Generic;

namespace SimplePipeline
{
    public interface IPipeline : IEnumerable<IFilter>
    {
        Object Output { get; }

        Exception Exception { get; }

        Boolean Execute(Object input);

        void Reset();
    }

    public interface IPipeline<in TInput, out TOutput> : IPipeline
    {
        new TOutput Output { get; }

        Boolean Execute(TInput input);
    }
}