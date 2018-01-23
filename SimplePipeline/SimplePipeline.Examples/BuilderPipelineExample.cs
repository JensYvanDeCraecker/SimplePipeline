using System;
using SimplePipeline.Builder;

namespace SimplePipeline.Examples
{
    public static class BuilderPipelineExample
    {
        public static void Demo()
        {
            BuilderPipeline<String, String> pipeline = BuilderPipeline.Chain(Filter.Create<String, String>(input => input.ToUpper())).Chain(Filter.Create<String, String>(input => input.Trim()));
        }
    }
}