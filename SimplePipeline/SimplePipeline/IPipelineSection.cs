using System;
using System.Collections.Generic;

namespace SimplePipeline
{
    public interface IPipelineSection : IEnumerable<IFilter>
    {
        IPipelineSection Chain(IFilter filter);

        IPipelineSection Chain(Func<Object, Object> filter);

        IPipeline Build();
    }
}