namespace SimplePipeline.Generic
{
    public interface IPipelineBuilder<TPipeInput> : IPipelineBuilder
    {
        new IPipelineSection<TPipeInput, TPipeInput> Start();
    }
}