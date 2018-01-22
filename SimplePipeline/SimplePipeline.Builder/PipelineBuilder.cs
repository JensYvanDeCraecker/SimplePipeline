using System;

namespace SimplePipeline.Builder
{
    public class PipelineBuilder : IPipelineBuilder
    {
        public IPipelineSection Start()
        {
            return new PipelineSection();
        }
    }

    public class PipelineBuilder<TPipelineInputBase> : IPipelineBuilder<TPipelineInputBase>
    {
        public IPipelineSection<TPipelineInput, TPipelineInput> Start<TPipelineInput>() where TPipelineInput : TPipelineInputBase
        {
            return new PipelineSection<TPipelineInput>();
        }

        public IPipelineSection Start()
        {
            return Start<TPipelineInputBase>();
        }
    }
}