using System;

namespace SimplePipeline
{
    public interface IFilter
    {
        Object Execute(Object input);
    }
}