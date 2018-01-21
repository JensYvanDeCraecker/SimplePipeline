using System;

namespace SimplePipeline
{
    public interface IFilter
    {
        Object Execute(Object input);
    }

    public interface IFilter<in TInput, out TOutput> : IFilter
    {
        TOutput Execute(TInput input);
    }
}