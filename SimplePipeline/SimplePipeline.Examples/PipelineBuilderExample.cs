using System;
using SimplePipeline.Builder;

namespace SimplePipeline.Examples
{
    public static class PipelineBuilderExample
    {
        public static void Load()
        {
            IPipeline<String, Int32> pipeline = PipelineBuilder.Create<String, Int32>(builder => builder.Chain(input => input.Trim()).Chain(input => input.Length));
            if (pipeline.Execute(" This is an example "))
                Console.WriteLine(pipeline.Output);
        }
    }
}