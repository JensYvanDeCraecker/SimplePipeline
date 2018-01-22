using System;

namespace SimplePipeline.Builder
{
    public static class PipelineBuilder
    {
        public static IPipelineBuilder Create()
        {
            return Create<Object>();
        }

        public static IPipelineBuilder<TPipelineInputBase> Create<TPipelineInputBase>()
        {
            return new PipelineBuilder<TPipelineInputBase>();
        }
    }

    public class PipelineBuilder<TPipelineInputBase> : IPipelineBuilder<TPipelineInputBase>
    {
        public IPipelineSection<TPipelineInput, TPipelineInput> Start<TPipelineInput>() where TPipelineInput : TPipelineInputBase
        {
            return PipelineSection.Create<TPipelineInput>();
        }

        public IPipelineSection Start()
        {
            return Start<TPipelineInputBase>();
        }
    }
}